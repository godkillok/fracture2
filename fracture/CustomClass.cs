public class CustomClass
    {
        public delegate void ChangedEventHandler();//定义委托
        public event ChangedEventHandler Changed;//定义事件
        private int _Cid;
        private string _Cname;
 
 
        public CustomClass()
        {

        }
 

        public CustomClass(int cCid, string cCname)
        {
            this._Cid = cCid;
            this._Cname = cCname;
        }
 
 
        protected virtual void OnChanged()
        {
            if (Changed!=null)
            {
                Changed();
            }
        }
 
 
        public int Cid
        {
            get
            {
                return _Cid;
            }
            set
            {
                if (_Cid!=value)//这里是文本改变时的处理
                {
                    _Cid = value;
                    OnChanged();//启动事件
                    //注：变相的在更改值的过程中，调用了上面绑定的事件函数“cc_Changed()”，做到了对自定义事件的触发（cc_Changed()幻术的执行）         
                    //需要明白：在当前类CustomClass中，不能直接知道要执行的事件函数
                    //或者这么说：在定义自定义事件的类中，仅仅定义（1）事件本身及（2）可能在类中触发的事件调用（以本类而言，就是执行OnChanged()中的Changed()），具体调用哪个事件函数，是由“调用者端”提供的
                }
            }
        }
 
        public string Cname
        {
            get
            {
                return _Cname;
            }
            set
            {
                if (_Cname != value)
                {
                    _Cname = value;
                    OnChanged();
                }
            }
        }
    }
