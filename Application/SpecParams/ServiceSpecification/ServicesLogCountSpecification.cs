using System.Globalization;
using Application.SpecParams.ProductSpecification;
using Domain.Entity;

namespace Application.SpecParams.ServiceSpecification
{
    public class ServicesLogCountSpecification : BaseSpecification<ServiceLog>
    {
        public ServicesLogCountSpecification(ServiceLogSpecParams baseSpecParam) : base(
            x => string.IsNullOrEmpty(baseSpecParam.Search) ||
            (
                x.ServiceName.ToLower().Contains(baseSpecParam.Search)
            // || x.Description.ToLower().Contains(baseSpecParam.Search)
            )
            && (baseSpecParam.CourtCluster == null
            || baseSpecParam.CourtCluster == 0
            || x.CourtCluster.Id.Equals(baseSpecParam.CourtCluster)
            || x.CourtCluster.Id.Equals(baseSpecParam.CourtCluster)
            )
            && (
                baseSpecParam.LogType == null ||
                (int)x.LogType == baseSpecParam.LogType
                )
                && (
                    baseSpecParam.fromDate == null && baseSpecParam.toDate == null ||

                    x.CreatedAt >= DateTime.ParseExact(baseSpecParam.fromDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                    && x.CreatedAt <= DateTime.ParseExact(baseSpecParam.toDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                )
        )

        {

        }
    }
}