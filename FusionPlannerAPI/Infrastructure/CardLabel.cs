namespace FusionPlannerAPI.Infrastructure
{
    public class CardLabel
    {
        public int CardId { get; set; }
        public int LabelId { get; set; }

        public Card Card { get; set; }
        public Label Label { get; set; }
    }
}
