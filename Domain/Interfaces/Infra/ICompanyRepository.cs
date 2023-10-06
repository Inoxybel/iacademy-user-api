﻿using Domain.DTO;
using Domain.Entities;

namespace Domain.Interfaces.Infra;

public interface ICompanyRepository
{
    Task<Company> GetById(string companyId, CancellationToken cancellationToken = default);
    Task<Company> GetByRef(string Cnpj, CancellationToken cancellationToken = default);
    Task<string> Save(CompanyRequest request, CancellationToken cancellationToken = default);
    Task<bool> Update(string companyId, CompanyRequest request, CancellationToken cancellationToken = default);
}
