﻿namespace TRMApi.Models
{
    public record UserRegistrationModel(string FirstName,
                                        string LastName,
                                        string EmailAddress,
                                        string Password);
}
