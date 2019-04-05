[
    {
        "name": "H1",
        "alias": "h1",
        "view": "textstring",
        "icon": "icon-coin color-blue",
        "config": {
            "style": "font-size: 36px; line-height: 45px; font-weight: bold",
            "markup": "<h1>#value#</h1>"
        }
    },
    {
        "name": "H2",
        "alias": "h2",
        "view": "textstring",
        "icon": "icon-coin color-blue",
        "config": {
            "style": "font-size: 27px; letter-spacing: 2px; line-height: 37px;margin-bottom: 10px;",
            "markup": "<h2>#value#</h2>"
        }
    },
    {
        "name": "H3",
        "alias": "h3",
        "view": "textstring",
        "icon": "icon-coin color-blue",
        "config": {
            "markup": "<h3>#value#</h3>"
        }
    },
    {
        "name": "Link Button",
        "alias": "link",
        "view": "/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html",
        "icon": "icon-hand-pointer-alt color-blue",
        "render": "/App_Plugins/LeBlender/editors/leblendereditor/views/Base.cshtml",
        "config": {
            "editors": [
                {
                    "name": "link button",
                    "alias": "linkButton",
                    "propretyType": {},
                    "dataType": "69106773-bed4-4ea7-9e2b-325c24dee809"
                },
                {
                    "name": "Name button",
                    "alias": "nameButton",
                    "propretyType": {},
                    "dataType": "0cc0eba1-9960-42c9-bf9b-60e150b429ae"
                },
                {
                    "name": "align button",
                    "alias": "alignButton",
                    "propretyType": {},
                    "dataType": "0cc0eba1-9960-42c9-bf9b-60e150b429ae"
                }
            ],
            "renderInGrid": "1",
            "frontView": "",
            "min": null,
            "max": 1
        }
    },
    {
        "name": "Rich text editor",
        "alias": "rte",
        "view": "rte",
        "icon": "icon-article"
    },
    {
        "name": "Image Original",
        "alias": "media",
        "view": "media",
        "icon": "icon-picture"
    },
    {
        "name": "Image Square - Cropped",
        "alias": "mediasquare",
        "view": "media",
        "icon": "icon-crop color-orange",
        "config": {
            "size": {
                "width": 800,
                "height": 800
            }
        }
    },
    {
        "name": "Image vertical - Cropped",
        "alias": "mediavertical",
        "view": "media",
        "icon": "icon-crop color-orange",
        "config": {
            "size": {
                "width": 533,
                "height": 768
            }
        }
    },
    {
        "name": "Image Horizontal - Cropped",
        "alias": "mediahorizontal",
        "view": "media",
        "icon": "icon-crop color-orange",
        "config": {
            "size": {
                "width": 768,
                "height": 533
            }
        }
    },
    {
        "name": "Macro",
        "alias": "macro",
        "view": "macro",
        "icon": "icon-settings-alt"
    },
    {
        "name": "Embed",
        "alias": "embed",
        "view": "embed",
        "icon": "icon-movie-alt"
    },
    {
        "name": "Quote",
        "alias": "quote",
        "view": "textstring",
        "icon": "icon-quote",
        "config": {
            "style": "border-left: 3px solid #ccc; padding: 10px; color: #ccc; font-family: serif; font-style: italic; font-size: 18px",
            "markup": "<blockquote>#value#</blockquote>"
        }
    },
    {
        "name": "Headline",
        "alias": "headline",
        "view": "textstring",
        "icon": "icon-coin",
        "config": {
            "style": "font-size: 36px; line-height: 45px; font-weight: bold",
            "markup": "<h1>#value#</h1>"
        }
    },
    {
        "name": "Divider",
        "alias": "Divider whitespace",
        "view": "textstring",
        "icon": "icon-indent color-green",
        "config": {
            "markup": "<hr>",
            "style": "margin:0px;"
        }
    },
    {
        "name": "Featured products",
        "alias": "featuredProducts",
        "view": "/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html",
        "icon": "icon-pictures-alt-2 color-orange",
        "render": "/App_Plugins/LeBlender/editors/leblendereditor/views/Base.cshtml",
        "config": {
            "frontView": "",
            "editors": [
                {
                    "name": "Product picker",
                    "alias": "productPicker",
                    "propretyType": {},
                    "dataType": "56496381-97df-4862-ba3a-ac4e3b4506d6"
                }
            ],
            "renderInGrid": "1"
        }
    }
]