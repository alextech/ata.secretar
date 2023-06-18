using System;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Application.Households
{
    public static class HouseholdExtension
    {
        public static Client? GetClientById(this Household household, Guid clientId)
        {
            if (household.PrimaryClient.Guid.Equals(clientId))
            {
                return household.PrimaryClient;
            }

            if (household.IsJoint && (household.JointClient?.Guid.Equals(clientId) ?? false))
            {
                return household.JointClient;
            }

            return null;
        }
    }
}