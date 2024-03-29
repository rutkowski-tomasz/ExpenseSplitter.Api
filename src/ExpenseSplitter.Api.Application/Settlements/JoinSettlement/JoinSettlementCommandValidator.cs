﻿using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Settlements.JoinSettlement;

[ExcludeFromCodeCoverage]
public sealed class JoinSettlementCommandValidator : AbstractValidator<JoinSettlementCommand>
{
    public JoinSettlementCommandValidator()
    {
        RuleFor(x => x.InviteCode).NotEmpty();
    }
}
