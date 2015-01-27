namespace MongoDB_Sample.Model
{
    using System.Collections.Generic;

    public class RspUser : RspFrame
    {
        public IList<RspUserResult> result { get; set; }
    }
    public class RspUserResult
    {
        public object Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}



