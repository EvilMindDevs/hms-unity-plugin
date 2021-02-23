namespace HmsPlugin
{
    // Helper wrapper class for any Sequence that allows to draw something else when Sequence is empty
    // Draws nothing if nothings was set to draw
    public class EmptySequenceDrawer : IDrawer
    {
        private readonly SequenceDrawer _sequenceDrawer;
        private IDrawer _emptyDrawer;

        public EmptySequenceDrawer(SequenceDrawer sequenceDrawer)
        {
            _sequenceDrawer = sequenceDrawer;
        }

        public EmptySequenceDrawer SetEmptyDrawer(IDrawer drawer)
        {
            _emptyDrawer = drawer;
            return this;
        }

        public void Draw()
        {
            if (_sequenceDrawer.Count() > 0)
            {
                _sequenceDrawer.Draw();
            }
            else if (_emptyDrawer != null)
            {
                _emptyDrawer.Draw();
            }
        }
    }
}