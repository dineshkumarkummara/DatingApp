using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Interfaces;
using DatingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace DatingApp.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public ITokenService _tokenServices { get; }

        public AccountController(DataContext context, ITokenService tokenService){
            _context=context;
            _tokenServices=tokenService;
        }
        [HttpPost("register")]   //post:api/account/register
        public async Task<ActionResult<UserDto>> register(RegisterDto registerDto){
            if(await userExists(registerDto.UserName)) return BadRequest("username already exists");
            using var hmac=new HMACSHA512();
            var user=new AppUser{
                UserName=registerDto.UserName.ToLower(),
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                PasswordSalt=hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto{
                userName=user.UserName,
                token=_tokenServices.CreateToken(user),
            };
        }

        private async Task<bool> userExists(string username){
            return await _context.Users.AnyAsync(x=>x.UserName.ToLower()==username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
            var user= await _context.Users.SingleOrDefaultAsync(x=>x.UserName ==loginDto.UserName);
            if(user==null) return Unauthorized("invalid username");
            using var hmac=new HMACSHA512(user.PasswordSalt);
            var ComputeHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));
            for (int i = 0; i < ComputeHash.Length; i++)
            {
                if(ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }
            return new UserDto{
                userName=user.UserName,
                token=_tokenServices.CreateToken(user),
            };
        } 
    }
}