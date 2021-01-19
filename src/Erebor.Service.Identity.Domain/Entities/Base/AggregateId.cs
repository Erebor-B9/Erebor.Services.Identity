﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erebor.Service.Identity.Domain.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Erebor.Service.Identity.Domain.Entities.Base
{
    public class AggregateId
    {
        [BsonId]
        public Guid Value { get; }

        public AggregateId()
        {
            //???
            //Value = Guid.NewGuid();
        }

        public AggregateId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new InvalidAggregateIdException("Invalid AggregateId Exception");
            }

            Value = value;
        }

        public bool Equals(AggregateId other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((AggregateId)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator Guid(AggregateId id)
            => id.Value;

        public static implicit operator AggregateId(Guid id)
            => new AggregateId(id);

        public override string ToString() => Value.ToString();
    }
}
