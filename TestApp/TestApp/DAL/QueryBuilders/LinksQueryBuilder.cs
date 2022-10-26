namespace TestApp.DAL.QueryBuilders;

public class LinksQueryBuilder : QueryBuilder<LinkRecord, ApplicationContext>
{
    public LinksQueryBuilder(ApplicationContext context) : base(context)
    {
    }
}