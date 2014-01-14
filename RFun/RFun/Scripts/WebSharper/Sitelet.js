(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,jQuery,WebSharper,Concurrency,Website,Search,Client,Html,Default,List,window,Operators,HTML5,EventsPervasives,Strings,Seq,Math,Operators1,T,Remoting;
 Runtime.Define(Global,{
  Website:{
   Search:{
    Client:{
     bindClick:function()
     {
      return jQuery("#search-btn-2").click(function()
      {
       var arg00,clo1,t;
       arg00=Concurrency.Delay((clo1=function()
       {
        var queryStr,x,f,x1,f1;
        jQuery("#results").empty();
        jQuery("#pagination").empty();
        jQuery("#progress").show();
        queryStr=(x=jQuery("#query-2").val(),(f=function(value)
        {
         return Global.String(value);
        },f(x)));
        x1=Client.search(queryStr,1);
        f1=Runtime.Tupled(function(_arg11)
        {
         var matches,results,alert,_this,x2,f2,patternInput,f3;
         matches=_arg11[0];
         results=_arg11[1];
         if(matches===0)
          {
           jQuery("#progress").hide();
           alert=Default.Div(List.ofArray([Default.Attr().Class("alert alert-danger"),Default.Text("Your search did not match any functions."),(_this=Default.Attr(),_this.NewAttr("style","width: 30%;"))]));
           x2=jQuery("#results").append(alert.Body);
           f2=function(value)
           {
            value;
           };
           f2(x2);
           return Concurrency.Return(null);
          }
         else
          {
           if(matches===1)
            {
             patternInput=results[0];
             f3=patternInput[0];
             window.location.assign("/function/"+f3);
             return Concurrency.Return(null);
            }
           else
            {
             jQuery("#progress").hide();
             Client.displayResults(results);
             Client.paginationDiv(matches,1,queryStr);
             jQuery(".page").first().addClass("active");
             return Concurrency.Return(null);
            }
          }
        });
        return Concurrency.Bind(x1,f1);
       },clo1));
       t={
        $:0
       };
       return Concurrency.Start(arg00);
      });
     },
     displayResults:function(results)
     {
      var lis,f,mapping,ul;
      lis=(f=(mapping=Runtime.Tupled(function(tupledArg)
      {
       var docId,name,desc,pack,x,_this,x1;
       docId=tupledArg[0];
       name=tupledArg[1];
       desc=tupledArg[2];
       pack=tupledArg[3];
       return Operators.add(Default.LI(List.ofArray([Default.Attr().Class("list-group-item")])),List.ofArray([Operators.add(Default.H3(List.ofArray([Default.Attr().Class("list-group-item-heading")])),List.ofArray([Operators.add(Default.A(List.ofArray([(x="/function/"+docId,Default.HRef(x)),(_this=Default.Attr(),_this.NewAttr("target","_blank"))])),List.ofArray([Default.Text(name)]))])),Default.Div(List.ofArray([Default.Text(desc)])),Default.H4(List.ofArray([(x1="Package: "+pack,Default.Text(x1))]))]));
      }),function(array)
      {
       return array.map(function(x)
       {
        return mapping(x);
       });
      }),f(results));
      ul=Operators.add(Default.UL(List.ofArray([Default.Attr().Class("col-lg-8")])),lis);
      return jQuery("#results").empty().append(ul.Body);
     },
     link:function(href,text)
     {
      return Operators.add(Default.A(List.ofArray([Default.HRef(href)])),List.ofArray([Default.Text(text)]));
     },
     main:function()
     {
      var inp,x,_this,_this1,_this2,_this3,f,arg00,x1,_this4,f1,arg001,_this6;
      inp=(x=Default.Input(List.ofArray([(_this=Default.Attr(),_this.NewAttr("id","query-1")),(_this1=Default.Attr(),_this1.NewAttr("type","text")),Default.Attr().Class("query form-control input-lg"),(_this2=HTML5.Attr(),_this2.NewAttr("autofocus","autofocus")),(_this3=HTML5.Attr(),_this3.NewAttr("placeholder","R Function"))])),(f=(arg00=function()
      {
       return function(key)
       {
        var matchValue;
        matchValue=key.KeyCode;
        if(matchValue===13)
         {
          jQuery("#query-1").blur();
          return jQuery("#search-btn-1").click();
         }
        else
         {
          return null;
         }
       };
      },function(arg10)
      {
       return EventsPervasives.Events().OnKeyUp(arg00,arg10);
      }),(f(x),x)));
      return Operators.add(Default.Div(List.ofArray([Default.Attr().Class("row")])),List.ofArray([Operators.add(Default.Div(List.ofArray([Default.Attr().Class("col-lg-6 col-lg-offset-3")])),List.ofArray([Operators.add(Default.Div(List.ofArray([Default.Attr().Class("input-group input-group-lg")])),List.ofArray([inp,Operators.add(Default.Span(List.ofArray([Default.Attr().Class("input-group-btn")])),List.ofArray([(x1=Default.Button(List.ofArray([Default.Id("search-btn-1"),Default.Text("Search"),(_this4=Default.Attr(),_this4.NewAttr("type","button")),Default.Attr().Class("btn btn-primary btn-lg")])),(f1=(arg001=function()
      {
       return function()
       {
        var q,_this5,x2,f2,f6;
        q=(_this5=inp.get_Value(),Strings.Trim(_this5));
        x2=(f2=function()
        {
         var x3,f3;
         jQuery("#progress").show();
         jQuery("#results").empty();
         jQuery("#pagination").empty();
         x3=Client.search(q,1);
         f3=Runtime.Tupled(function(_arg1)
         {
          var matches,results,alert,x4,f4,patternInput,f5,form;
          matches=_arg1[0];
          results=_arg1[1];
          if(matches===0)
           {
            jQuery("#progress").hide();
            alert=Default.Div(List.ofArray([Default.Attr().Class("alert alert-danger col-lg-offset-3 col-lg-6"),Default.Text("Your search did not match any functions.")]));
            x4=jQuery("#results").append(alert.Body);
            f4=function(value)
            {
             value;
            };
            f4(x4);
            return Concurrency.Return(null);
           }
          else
           {
            if(matches===1)
             {
              patternInput=results[0];
              f5=patternInput[0];
              window.location.assign("/function/"+f5);
              return Concurrency.Return(null);
             }
            else
             {
              jQuery("#progress").hide();
              form=Client.searchForm();
              jQuery(".jumbotron").replaceWith(form.Body);
              jQuery("#query-2").val(q);
              Client.displayResults(results);
              Client.paginationDiv(matches,1,q);
              jQuery(".page").first().addClass("active");
              Client.bindClick();
              return Concurrency.Return(null);
             }
           }
         });
         return Concurrency.Bind(x3,f3);
        },Concurrency.Delay(f2));
        f6=function(arg002)
        {
         var t;
         t={
          $:0
         };
         return Concurrency.Start(arg002);
        };
        return f6(x2);
       };
      },function(arg10)
      {
       return EventsPervasives.Events().OnClick(arg001,arg10);
      }),(f1(x1),x1)))]))])),Default.Script(List.ofArray([(_this6=Default.Attr(),_this6.NewAttr("src","/Scripts/AutoComplete.js"))]))]))]));
     },
     pageLi:function(x,pageId,queryStr)
     {
      var xStr,li;
      xStr=Global.String(x);
      li=Operators.add(Default.LI(List.ofArray([Default.Attr().Class("page")])),List.ofArray([Client.link("#",xStr)]));
      li.Body.addEventListener("click",function(e)
      {
       var objectArg,arg00;
       e.preventDefault();
       Client["search'"](queryStr,x);
       jQuery(".page").removeClass("active");
       objectArg=li["HtmlProvider@32"];
       return(arg00=li.Body,function(arg10)
       {
        return objectArg.AddClass(arg00,arg10);
       })("active");
      },false);
      return li;
     },
     pagesUl:function(pageId,queryStr,pages)
     {
      return Operators.add(Default.UL(List.ofArray([Default.Attr().Class("pagination")])),Seq.toList(Seq.delay(function()
      {
       return pages.map(function(x)
       {
        return function(x1)
        {
         return Client.pageLi(x1,pageId,queryStr);
        }(x);
       });
      })));
     },
     paginationDiv:function(matches,pageId,queryStr)
     {
      var pages,x,value,value1,length,_pages_,source,div,value2;
      pages=(x=(value=(value1=matches/10,Math.ceil(value1)),value<<0),Seq.toArray(Operators1.range(1,x)));
      length=pages.length;
      _pages_=(source=Seq.truncate(10,pages),Seq.toArray(source));
      div=length===1?Default.Div(Runtime.New(T,{
       $:0
      })):Operators.add(Default.Div(List.ofArray([Default.Attr().Class("row")])),List.ofArray([Client.pagesUl(pageId,queryStr,_pages_,length)]));
      value2=jQuery("#pagination").append(div.Body);
      value2;
     },
     search:function(queryStr,page)
     {
      var clo1;
      return Concurrency.Delay((clo1=function()
      {
       var start,x;
       start=(page-1)*10;
       x=Remoting.Async("Sitelet:0",[queryStr,start]);
       return x;
      },clo1));
     },
     "search'":function(queryStr,page)
     {
      var arg00,clo1,t;
      arg00=Concurrency.Delay((clo1=function()
      {
       var x,f;
       x=Client.search(queryStr,page);
       f=Runtime.Tupled(function(_arg1)
       {
        Client.displayResults(_arg1[1]);
        return Concurrency.Return(null);
       });
       return Concurrency.Bind(x,f);
      },clo1));
      t={
       $:0
      };
      return Concurrency.Start(arg00);
     },
     searchForm:function()
     {
      var x,_this,f,arg00,_this1,_this2;
      return Operators.add(Default.Div(List.ofArray([Default.Attr().Class("row"),Default.Id("search-form")])),List.ofArray([Operators.add(Default.Div(List.ofArray([Default.Attr().Class("input-group input-group-lg")])),List.ofArray([Operators.add(Default.Span(List.ofArray([Default.Attr().Class("input-group-addon")])),List.ofArray([Default.Text("RFun")])),(x=Default.Input(List.ofArray([(_this=Default.Attr(),_this.NewAttr("type","text")),Default.Attr().Class("form-control query"),Default.Id("query-2")])),(f=(arg00=function()
      {
       return function(key)
       {
        var matchValue;
        matchValue=key.KeyCode;
        if(matchValue===13)
         {
          jQuery("#query-2").blur();
          return jQuery("#search-btn-2").click();
         }
        else
         {
          return null;
         }
       };
      },function(arg10)
      {
       return EventsPervasives.Events().OnKeyUp(arg00,arg10);
      }),(f(x),x))),Operators.add(Default.Span(List.ofArray([Default.Attr().Class("input-group-btn")])),List.ofArray([Operators.add(Default.Button(List.ofArray([Default.Attr().Class("btn btn-primary"),(_this1=Default.Attr(),_this1.NewAttr("type","button")),Default.Id("search-btn-2")])),List.ofArray([Default.Text("Search")]))]))])),Default.Script(List.ofArray([(_this2=Default.Attr(),_this2.NewAttr("src","/Scripts/AutoComplete.js"))]))]));
     }
    },
    Control:Runtime.Class({
     get_Body:function()
     {
      return Client.main();
     }
    })
   }
  }
 });
 Runtime.OnInit(function()
 {
  jQuery=Runtime.Safe(Global.jQuery);
  WebSharper=Runtime.Safe(Global.IntelliFactory.WebSharper);
  Concurrency=Runtime.Safe(WebSharper.Concurrency);
  Website=Runtime.Safe(Global.Website);
  Search=Runtime.Safe(Website.Search);
  Client=Runtime.Safe(Search.Client);
  Html=Runtime.Safe(WebSharper.Html);
  Default=Runtime.Safe(Html.Default);
  List=Runtime.Safe(WebSharper.List);
  window=Runtime.Safe(Global.window);
  Operators=Runtime.Safe(Html.Operators);
  HTML5=Runtime.Safe(Default.HTML5);
  EventsPervasives=Runtime.Safe(Html.EventsPervasives);
  Strings=Runtime.Safe(WebSharper.Strings);
  Seq=Runtime.Safe(WebSharper.Seq);
  Math=Runtime.Safe(Global.Math);
  Operators1=Runtime.Safe(WebSharper.Operators);
  T=Runtime.Safe(List.T);
  return Remoting=Runtime.Safe(WebSharper.Remoting);
 });
 Runtime.OnLoad(function()
 {
 });
}());
