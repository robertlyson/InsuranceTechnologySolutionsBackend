namespace Claims.Auditing
{
    public class Auditer
    {
        private readonly AuditContext _auditContext;

        public Auditer(AuditContext auditContext)
        {
            _auditContext = auditContext;
        }

        public async Task AuditClaim(string id, string httpRequestType, CancellationToken cancellationToken)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.UtcNow,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            _auditContext.Add(claimAudit);
            await _auditContext.SaveChangesAsync(cancellationToken);
        }
        
        public async Task AuditCover(string id, string httpRequestType, CancellationToken cancellationToken)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.UtcNow,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            _auditContext.Add(coverAudit);
            await _auditContext.SaveChangesAsync(cancellationToken);
        }
    }
}
