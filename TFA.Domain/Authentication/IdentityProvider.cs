﻿using TFA.Domain.Identity;

namespace TFA.Domain.Authentication;

internal class IdentityProvider : IIdentityProvider
{
    public IIdentity Current { get; set; }
}
