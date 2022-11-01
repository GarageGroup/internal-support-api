﻿using GGroupp.Infra;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

partial class CustomerSetSearchFunc
{
    public ValueTask<Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>> InvokeAsync(
        CustomerSetSearchIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .HandleCancellation()
        .Pipe(
            static @in => new DataverseSearchIn($"*{@in.SearchText}*")
            {
                Entities = entities,
                Top = @in.Top 
            })
        .PipeValue(
            dataverseSearchSupplier.SearchAsync)
        .MapFailure(
            static failure => failure.MapFailureCode(MapFailureCode))
        .MapSuccess(
            static @out => new CustomerSetSearchOut(
                @out.Value.Select(MapCustomerItemSearchOut).ToFlatArray()));

    private static CustomerItemSearchOut MapCustomerItemSearchOut(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            title: item.ExtensionData.GetValueOrAbsent("name").OrDefault()?.ToString());

    private static CustomerSetSearchFailureCode MapFailureCode(DataverseFailureCode failureCode)
        =>
        failureCode switch
        {
            DataverseFailureCode.UserNotEnabled => CustomerSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.SearchableEntityNotFound => CustomerSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => CustomerSetSearchFailureCode.TooManyRequests,
            _ => default
        };
}