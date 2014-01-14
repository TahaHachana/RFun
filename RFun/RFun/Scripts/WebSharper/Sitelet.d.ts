declare module Website {
    module Site {
        interface Website {
        }
    }
    module Model {
        interface Action {
        }
    }
    module Search {
        interface Control {
            get_Body(): _Html.IPagelet;
        }
    }
    module Mongo {
        interface Function {
            _id: any;
            FunId: number;
            Name: string;
            Args: any[];
            Description: string;
            Reference: string;
            Package: string;
            Usage: string;
        }
        interface Arg {
            Name: string;
            Description: string;
        }
    }
    module Skin {
        interface Page {
            Title: string;
            MetaDesc: string;
            Body: _Html1.Element<_Web.Control>;
        }
    }
    
    import _Html = IntelliFactory.WebSharper.Html;
    import _Html1 = IntelliFactory.Html.Html;
    import _Web = IntelliFactory.WebSharper.Web;
}
