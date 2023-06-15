using System.Text;

namespace BackgroundFileLogging
{
    public class LoggingScope : IDisposable
    {
        private readonly List<string> _scopes;

        public LoggingScope()
        {
            _scopes = new List<string>();
        }

        public void AddScope(string scope)
        {
            _scopes.Add(scope);
        }

        public void Dispose()
        {
            if(_scopes.Count > 0) 
            {
                _scopes.RemoveAt(_scopes.Count - 1);
            }            
        }

        public override string ToString()
        {
            bool firstStringPrinted = false;
            StringBuilder stringBuilder = new StringBuilder();

            foreach(string scope in _scopes)
            {
                if(!firstStringPrinted)
                {
                    stringBuilder.Append(scope);
                    firstStringPrinted = true;
                }
                else
                {
                    stringBuilder.Append(" => ").Append(scope);
                }                
            }

            return stringBuilder.ToString();
        }
    }
}
