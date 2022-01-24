﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public readonly record struct IncidentCreateIn
{
    private readonly string? title, description;

    public IncidentCreateIn(
        Guid ownerId,
        Guid customerId,
        string title,
        [AllowNull] string description,
        int caseTypeCode,
        Guid? contactId = null,
        int? caseOriginCode = null)
    {
        OwnerId = ownerId;
        CustomerId = customerId;
        ContactId = contactId;
        this.title = title.OrNullIfEmpty();
        this.description = description.OrNullIfEmpty();
        CaseTypeCode = caseTypeCode;
        CaseOriginCode = caseOriginCode;
    }

    public Guid OwnerId { get; }

    public Guid CustomerId { get; }

    public Guid? ContactId { get; }

    public string Title => title.OrEmpty();

    public string Description => description.OrEmpty();

    public int CaseTypeCode { get; }

    public int? CaseOriginCode { get; }
}