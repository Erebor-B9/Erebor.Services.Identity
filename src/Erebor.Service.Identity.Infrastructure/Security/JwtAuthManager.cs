﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Erebor.Service.Identity.Core.Interfaces;
using Erebor.Service.Identity.Domain.Entities;
using Erebor.Service.Identity.Shared.Security;
using Microsoft.IdentityModel.Tokens;

namespace Erebor.Service.Identity.Infrastructure.Security
{
    public class JwtAuthManager : IJwtAuthManager
    {
        private readonly JwtAuthConfig _jwtTokenConfig;
        private readonly byte[] _secret;

        public JwtAuthManager(JwtAuthConfig jwtTokenConfig)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }
        public  Task<(string,DateTime,string)> GenerateTokens(string username, List<Role> claims, DateTime date)
        {
            var claimList = new List<Claim>()
            {
                new(ClaimTypes.Name,username)
            };

            claims.ForEach(role => { claimList.Add(new Claim(ClaimTypes.Role, role.Value)); });

            var jwtToken = new JwtSecurityToken(
                _jwtTokenConfig.Issuer,
               _jwtTokenConfig.Audience,
                claimList,
                expires: date.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var refreshToken =  TokenGenerator.GenerateRefreshToken();
            var tokenExpiredDate = date.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration);
            return Task.FromResult((accessToken, tokenExpiredDate, refreshToken));
        }

    }
}