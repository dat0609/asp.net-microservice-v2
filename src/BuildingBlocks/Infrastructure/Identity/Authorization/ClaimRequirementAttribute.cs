using Microsoft.AspNetCore.Mvc;
using Shared.Common.Constants;

namespace Infrastructure.Identity.Authorization;

public class ClaimRequirementAttribute : TypeFilterAttribute
{
    public ClaimRequirementAttribute(FunctionCode functionCode, CommandCode commandCode) : base(typeof(ClaimRequirementFilter))
    {
        Arguments = new object[] { functionCode, commandCode };
    }
}