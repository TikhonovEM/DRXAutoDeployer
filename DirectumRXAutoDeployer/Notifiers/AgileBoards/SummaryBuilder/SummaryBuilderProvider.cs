namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.SummaryBuilder
{
    public static class SummaryBuilderProvider
    {
        public static ISummaryBuilder GetBuilderByTarget(string summaryTarget)
        {
            if (HyperlinksUtils.IsConfigured())
                return new MarkdownTextSummaryBuilder();

            return new PlainTextSummaryBuilder();
        }
    }
}