using Microsoft.AspNetCore.Mvc;
using QL_HS.Database;
using QL_HS.Helper;
using System.Collections.Generic;
using System.Linq;
using System;
using QL_HS.Extensions;

namespace QL_HS.Base
{
    public abstract class QlhsControllerBase : ControllerBase
    {
        protected readonly QLHSDbContext _db;
        private AccountEntity _account;
        int DEFAULT_ROW_PER_PAGE = 20;
        public QlhsControllerBase(QLHSDbContext db)
        {
            _db = db;
        }
        public AccountEntity CurrentAccount
        {
            get
            {
                if (_account != null)
                {
                    return _account;
                }
                if (CurrentRole == UserRoles.ROLE_ROOT)
                {
                    return _account = new AccountEntity
                    {
                        Id = 0,
                        Role = CurrentRole,
                        Username = CurrentUsername
                    };
                }
                return _account = _db.Accounts.FirstOrDefault(e => e.Id == (int)JwtHelper.GetIdFromToken(User.Claims));
            }
        }

        public GuardianEntity? CurrentGuardian
        {
            get {
                return CurrentAccount?.Guardian;
            }
        }

        public string CurrentRole
        {
            get
            {
                return JwtHelper.GetCurrentInformation(User, e => e.Type.Equals(ClaimJwt.ROLE));
            }
        }

        public string CurrentUsername
        {
            get
            {
                return JwtHelper.GetCurrentInformation(User, e => e.Type.Equals(ClaimJwt.USERNAME));
            }
        }

        public int CurrentAccountId
        {
            get
            {
                return (int)JwtHelper.GetCurrentInformationLong(User, e => e.Type.Equals(ClaimJwt.ID));
            }
        }

        public bool IsRoot => CurrentRole == UserRoles.ROLE_ROOT;
        public bool IsAdmin => CurrentRole == UserRoles.ROLE_ADMIN;
        public bool IsMonitor => CurrentRole == UserRoles.ROLE_MONITOR;
        public bool IsAdminGroup => IsRoot || IsAdmin;


        public override OkObjectResult Ok(object value)
        {
            int.TryParse(HttpContext.Request.Query["page"].ToString(), out int page);
            int.TryParse(HttpContext.Request.Query["rowPerPage"].ToString(), out int rowPerPage);
            string keyword = HttpContext.Request.Query["keyword"].ToString();
            if (rowPerPage == 0)
            {
                rowPerPage = DEFAULT_ROW_PER_PAGE;
            }

            var queries = HttpContext.Request.Query;
            Dictionary<string, object> query = new Dictionary<string, object>();
            foreach (var item in queries)
            {
                if (item.Key != "page" && item.Key != "rowPerPage")
                    query.Add(item.Key, item.Value);
            }
            var response = new ActionResultDto
            {
                ErrorCode = 200,
                ErrorDesc = "Thành công",
                Data = value,
                Meta = new ActionResultMetaDto
                {
                    CurrentPage = page,
                    RowPerPage = rowPerPage,

                    Query = query
                }
            };
            if (value is ActionResultDto)
            {
                var rs = value as ActionResultDto;
                response.Data = rs.Data;
                response.Meta.TotalItem = rs.Meta.TotalItem;
                response.Meta.TotalPage = StringHelper.StaticCountTotalPage(rs.Meta.TotalItem, rowPerPage);
            }
            return new OkObjectResult(response);
        }

        public IQueryable<T> ApplyPagination<T>(IQueryable<T> source)
        {
            return source.Skip((Page - 1) * RowPerPage).Take(RowPerPage);
        }

        public IQueryable<T> ApplyPaginationWithOrderBy<T>(IQueryable<T> rs)
        {
            var orderBy = HttpContext.Request.Query["orderBy"].ToString();

            if (!string.IsNullOrEmpty(orderBy))
            {
                var orderType = "desc";
                if (!string.IsNullOrEmpty(HttpContext.Request.Query["orderType"]))
                {
                    orderType = HttpContext.Request.Query["orderType"].ToString();
                }
                rs = rs.OrderByField(orderBy, orderType);
            }

            var data = ApplyPagination(rs);

            return data;
        }

        public OkObjectResult OkList<T>(IQueryable<T> rs)
        {
            var response = new ActionResultDto
            {
                Data = ApplyPaginationWithOrderBy(rs).ToList(),
                Meta = new ActionResultMetaDto
                {
                    TotalItem = rs.Count()
                }
            };

            return Ok(response);
        }

        public OkObjectResult OkList<TEntity, TDto>(IQueryable<TEntity> rs, Func<TEntity, TDto> transformFunc)
        {
            var entities = ApplyPaginationWithOrderBy(rs).ToList();
            var result = entities.Select(transformFunc);

            var response = new ActionResultDto
            {
                Data = result,
                Meta = new ActionResultMetaDto
                {
                    TotalItem = rs.Count()
                }
            };

            return Ok(response);
        }

        
        public OkObjectResult OkList<T>(List<T> rs, int? count = null)
        {
            var response = new ActionResultDto
            {
                Data = rs,
                Meta = new ActionResultMetaDto
                {
                    TotalItem = count != null ? count.Value : rs.Count()
                }
            };

            return Ok(response);
        }


        public OkObjectResult OkList<T>(IEnumerable<T> rs)
        {
            var data = rs.Skip((Page - 1) * RowPerPage).Take(RowPerPage);
            var response = new ActionResultDto
            {
                Data = data,
                Meta = new ActionResultMetaDto
                {
                    TotalItem = rs.Count()
                }
            };
            return Ok(response);
        }

        public override CreatedAtActionResult CreatedAtAction(string actionName, object routeValues, object value)
        {
            var response = new ActionResultDto
            {
                ErrorCode = 1,
                ErrorDesc = "Thành công",
                Data = value
            };
            return CreatedAtAction(actionName, null, routeValues, response);
        }

        public int Page
        {
            get
            {
                int.TryParse(HttpContext.Request.Query["page"].ToString(), out int page);
                return page > 0 ? page : 1;
            }
        }

        public int RowPerPage
        {
            get
            {
                int.TryParse(HttpContext.Request.Query["rowPerPage"].ToString(), out int rowPerPage);
                return rowPerPage == 0 ? DEFAULT_ROW_PER_PAGE : rowPerPage;
            }
        }
    }

}
