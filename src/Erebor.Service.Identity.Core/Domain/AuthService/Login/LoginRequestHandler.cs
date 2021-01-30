﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Erebor.Service.Identity.Core.Exceptions;
using Erebor.Service.Identity.Core.Interfaces;
using Erebor.Service.Identity.Domain.Entities;
using Erebor.Service.Identity.Domain.Repositories;
using Erebor.Service.Identity.Shared.Security;
using MediatR;

namespace Erebor.Service.Identity.Core.Domain.AuthService.Login
{
    public class LoginRequestHandler : IRequestHandler<LoginRequest, LoginResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtAuthManager _jwtTokenManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public LoginRequestHandler(IUserRepository userRepository, IJwtAuthManager jwtTokenManager, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _jwtTokenManager = jwtTokenManager;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<LoginResult> Handle(LoginRequest request, CancellationToken cancellationToken)
        {

            var user = await _userRepository.GetUserByNameAsync(request.UserName);
            if (user is null)
                throw new ServiceException("User not found");
            var isValidPassword = PasswordHelper.Check(user.Password, request.Password);
            if (!isValidPassword)
                throw new ServiceException("Incorrect Password");

            var token = await _jwtTokenManager.GenerateTokens(user.UserName, user.Roles, DateTime.Now);
            await _refreshTokenRepository.AddAsync(Identity.Domain.Entities.RefreshToken.CreateRefreshToken(user.Id, token.Item3, DateTime.Now,
               DateTime.Now));
            return new LoginResult()
            {
                Token = token.Item1,
                TokenExpireDate = token.Item2,
                RefreshToken = token.Item3
            };
        }
    }
}