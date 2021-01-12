﻿using Erebor.Service.Identity.Domain.Entities.Base;
using Erebor.Service.Identity.Domain.Events;
using Erebor.Service.Identity.Domain.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erebor.Service.Identity.Domain.Entities
{
    public class User : Entity, IAggregateRoot
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<Email> Emails { get; set; }
        public List<Role> Roles { get; private set; }
        public string Password { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }
       
        protected User(List<Email> emails, List<Role> roles, string password, DateTime createdAt)
        {
           
            Emails = emails;
            Roles = roles;
            IsActive = true;
            CreatedAt = createdAt;
            AddEvent(new CreateUserEvent(roles, emails, password, createdAt));
        }
        public static User CreateUser(List<Email> emails, List<Role> roles, string password, DateTime createdAt)
            => new User(emails, roles, password, createdAt);
        public User RemoveMail(string email)
        {
            var mail = Emails.FirstOrDefault(x => x.Value == email);
            Emails.Remove(mail);
            return this;
        }
        public User AddMail(Email email)
        {
            if (Emails.Any(x => x.Value == email.Value))
            {
                throw new BusinessException("Email is already exist!");
            }
            Emails.Add(email);
            return this;
        }
        public User UpdateMail(string email, string value)
        {
            var mail = Emails.FirstOrDefault(x => x.Value == email);
            if (mail == null)
            {
                throw new BusinessException("Email could not find!");
            }
            mail.Value = value;
            return this;
        }
        public User ClearUserMailList()
        {
            Emails.Clear();
            return this;
        }
        public User AddRole(List<Role> roles)
        {
            var IsValid = Role.IsValid(roles);
            if (!IsValid)
                throw new BusinessException("Role is not valid!");
            else
                roles.ForEach(role =>
                {
                    if (Roles.Any(x => x.Value == role.Value))
                    {
                        throw new BusinessException($"This {role.Value} role is already given the user!");
                    }
                    Roles.Add(role);
                });
            AddEvent(new AddRoleEvent(roles));
            return this;
        }
        public User RemoveRoles(List<Role> roles)
        {
            var IsValid = Role.IsValid(roles);
            if (IsValid)
            {
                roles.ForEach(role =>
                {
                    var subRole = Roles.FirstOrDefault(role => role.Value == role.Value);
                    Roles.Remove(subRole);
                });
            }
            AddEvent(new RemoveRolesEvent(roles));
            return this;
        }
        public User UpdateRole(Role role, string value)
        {
            var IsValid = Role.IsValid(role);
            if (IsValid)
            {
                var userRole = Roles.FirstOrDefault(role => role.Value == role.Value);
                userRole.Value = value;
            }
            AddEvent(new UpdateRoleEvent(value, role));
            return this;
        }
        public User UpdatePassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
                Password = password;
            AddEvent(new UpdatePasswordEvent(password));
            return this;
        }
        public User ActivateUser()
        {
            IsActive = true;
            AddEvent(new ActivateUserEvent(IsActive));
            return this;
        }
        public User DeActivateUser()
        {
            IsActive = false;
            AddEvent(new DeActivateUserEvent(IsActive));
            return this;
        }

    }
}
