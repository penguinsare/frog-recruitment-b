using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using crm.Authentication;
using crm.Data;
using crm.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System.IO;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using crm.BindingModels;

namespace crm.Controllers
{

    [Route("/api/[controller]")]
    [Authorize(Policy ="AdminPolicy")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CrmContext _context;
        private readonly ConfigurationDbContext _configContext;
        public UsersController(
            UserManager<ApplicationUser> userManager,
            CrmContext context,
            ConfigurationDbContext configContext)
        {
            _userManager = userManager;
            _context = context;
            _configContext = configContext;
        }

        // GET api/users
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OutputUser>>> GetUsers()
        {            


            var filteredUsers = _userManager.Users.Where(user => user.UserName != DefinedAdmin.UserName).Include(u => u.Recruiter);
            try
            {
                var users = filteredUsers.AsEnumerable()
               .Select(user => new OutputUser
               {
                   UserName = user.UserName,
                   Recruiter = new OutputRecruiter
                   {
                       Id = user.Recruiter == null ? 0 : user.Recruiter.RecruiterId,
                       Name = user.Recruiter == null ? "" : user.Recruiter.Name,
                       Email = user.Recruiter == null ? "" : user.Recruiter.Email,
                       Phone = user.Recruiter == null ? "" : user.Recruiter.Phone
                   },
               });
               IList<OutputUser> outputUsers = new List<OutputUser>();
                foreach (var u in users)
                {
                    var appUser = await _userManager.FindByNameAsync(u.UserName);
                    var claims = await _userManager.GetClaimsAsync(appUser);
                    u.AccessPermission = claims.FirstOrDefault(claim => claim.Type == DefinedClaimTypes.Access).Value;
                    u.CanRemoveUser = !(_context.Jobs.Any(j => j.RecruiterId == u.Recruiter.Id) ||
                        _context.CrmClients.Any(cl => cl.RecruiterId == u.Recruiter.Id) ||
                        _context.Candidates.Any(c => c.RecruiterId == u.Recruiter.Id));

                    outputUsers.Add(u);
                }
               //.ToListAsync();
                return Ok(outputUsers);
            }
            catch (Exception ex)
            {
                var debugEx = ex;
            }
            return BadRequest();
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] RegisterUser registerUserModel)
        {
            try
            {
                if (!ModelState.IsValid)                
                    return BadRequest();

                if (registerUserModel.Password != registerUserModel.ConfirmPassword)
                    return BadRequest();

                if (String.IsNullOrEmpty(registerUserModel.Name) || String.IsNullOrEmpty(registerUserModel.Email))
                    return BadRequest();

                var recruiter = new Recruiter()
                {
                    Email = registerUserModel.Email,
                    Name = registerUserModel.Name,
                    Phone = registerUserModel.Phone,
                };

                _context.Recruiters.Add(recruiter);
                await _context.SaveChangesAsync();
                var newUser = new ApplicationUser() { UserName = registerUserModel.UserName, Recruiter = recruiter };
                var result = await _userManager.CreateAsync(newUser, registerUserModel.Password);
                if (result.Succeeded)
                {
       
                    result = _userManager.AddClaimsAsync(newUser, new Claim[]{
                        new Claim(DefinedClaimTypes.RecruiterId, newUser.Recruiter.RecruiterId.ToString()),                       
                        new Claim(JwtClaimTypes.Name, newUser.Recruiter.Name),
                        new Claim(DefinedClaimTypes.Access, registerUserModel.AccessClaim),                        
                    }).Result;

                    if (!String.IsNullOrEmpty(registerUserModel.Email))
                        await _userManager.SetEmailAsync(newUser, registerUserModel.Email);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                var debugEx = ex;
                return BadRequest();
            }                                                       
        }

       
        
        // PUT api/accounts/5
        
        [HttpPut("{username}")]
        public async Task<ActionResult> Put(string userName, [FromBody] RegisterUser changeUser)
        {
            try
            {
                if (userName != changeUser.UserName)
                    return BadRequest();
                string newClaimValue;
                if (changeUser.AccessClaim == DefinedClaimAccessValues.Normal)
                {
                    newClaimValue = DefinedClaimAccessValues.Normal;
                }
                else if (changeUser.AccessClaim == DefinedClaimAccessValues.Elevated)
                {
                    newClaimValue = DefinedClaimAccessValues.Elevated;
                }
                else
                {
                    return BadRequest();
                }

                var user =  _userManager.Users.Include(u => u.Recruiter).FirstOrDefault(u => u.UserName == changeUser.UserName); 

                if (user == null)
                    return BadRequest();
                var oldEmail = await _userManager.GetEmailAsync(user);                
                if (oldEmail != changeUser.Email)
                {
                    var emailChangeToken = await _userManager.GenerateChangeEmailTokenAsync(user, changeUser.Email);
                    await _userManager.ChangeEmailAsync(user, changeUser.Email, emailChangeToken);
                }
                    
                var recruiter = _context.Recruiters.FirstOrDefault(r => r.RecruiterId == user.Recruiter.RecruiterId);
                recruiter.Name = changeUser.Name;
                recruiter.Email = changeUser.Email;
                recruiter.Phone = changeUser.Phone;
                var claims = await _userManager.GetClaimsAsync(user);
                var oldClaim = claims.FirstOrDefault(claim => claim.Type == DefinedClaimTypes.Access);
                
                await _userManager.ReplaceClaimAsync(user, oldClaim, new Claim(DefinedClaimTypes.Access, newClaimValue));
                _context.Entry(recruiter).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                var debugEx = ex;
                return BadRequest();
            }
        }

        
        [HttpPost("transfer-assets")]
        public async Task<ActionResult> Put([FromBody] BindTransferAssets transfer)
        {
            try
            {
                if (transfer.FromRecruiterId < 1 || transfer.ToRecruiterId < 1)
                    return BadRequest();

                var toRecruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.RecruiterId == transfer.ToRecruiterId);

                if (toRecruiter == null)
                    return BadRequest();

                var jobs = _context.Jobs.Where(j => j.RecruiterId == transfer.FromRecruiterId);
                foreach (var job in jobs)
                {
                    job.Recruiter = toRecruiter;
                }

                var clients = _context.CrmClients.Where(cl => cl.RecruiterId == transfer.FromRecruiterId);
                foreach (var client in clients)
                {
                    client.Recruiter = toRecruiter;
                }

                var candidates = _context.Candidates.Where(c => c.RecruiterId == transfer.FromRecruiterId);
                foreach (var candidate in candidates)
                {
                    candidate.Recruiter = toRecruiter;
                }

                await _context.SaveChangesAsync();                

                return NoContent();
            }
            catch (Exception ex)
            {
                var debugEx = ex;
                return BadRequest();
            }
            //return BadRequest();
        }

        [HttpPut("change-password")]
        public async Task<ActionResult> Put([FromBody] BindChangePassword changeUser)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(changeUser.UserName);
                if (user == null)
                    return BadRequest();
                if (changeUser.Password != changeUser.ConfirmPassword)
                    return BadRequest();

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, changeUser.Password);

                if (result.Succeeded)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE api/accounts/5
        [HttpDelete("{username}")]
        public async Task<ActionResult> Delete(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    if (_context.Jobs.Any(j => j.RecruiterId == user.RecruiterId) ||
                        _context.CrmClients.Any(cl => cl.RecruiterId == user.RecruiterId) ||
                        _context.Candidates.Any(c => c.RecruiterId == user.RecruiterId))
                        return BadRequest();

                    //var userRoles = await _userManager.GetRolesAsync(user);
                    if (user.UserName == DefinedAdmin.UserName)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        var recruiter = _context.Recruiters.FirstOrDefault(r => r.RecruiterId == user.RecruiterId);
                        _context.Recruiters.Remove(recruiter);
                        _context.SaveChanges();
                        await _userManager.DeleteAsync(user);
                        return NoContent();
                    }                    
                }                
                return NoContent();                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}