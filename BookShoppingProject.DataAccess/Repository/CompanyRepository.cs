﻿using BookShoppingProject.DataAccess.Data;
using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context):base(context)
        {
            _context = context;

        }
        public void Update(Company company)
        {
            _context.Companies.Update(company);
        }
    }
}
