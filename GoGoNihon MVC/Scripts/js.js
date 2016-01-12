$(document).ready(function () {

    $('.bxslider').bxSlider({
        easing: 'cubic-bezier(.01,-0.00,.12,.98)',
        speed: 1200,
        auto: true
    });

    //$('.bxslider li').css("display", "list-item");

    $("[data-close]").on("click", function () {
        $(this).closest(".closeable").slideUp(700, function () {
            $(this).remove();
        })
    })


    //bind forms with data-ajax to sbumit via the ajax function
    $("[data-ajax]").on("submit", function (event) {
        console.log("data-ajax hit");
        event.preventDefault();
        var form = this;

        var method = $(form).attr("method");
        var action = $(form).attr("action");

        if (typeof tinymce != 'undefined') {
            for (var i = 0; i < tinymce.editors.length; i++) {
                tinymce.editors[i].save();
            }
        }
        
        

        var formData = new FormData($(this)[0]);

        ajaxRequest(method, action, formData, null);

    });


    $(".edit-click span").on("click", function (event) {
        event.stopPropagation();
        event.preventDefault();
        $(this).parent("form").submit();
    });

    $("[data-displayeditnodata]").on("click", function () {
        displayEditModalNoData($(this).attr("data-contentID"), $(this).attr("data-name"), $(this).attr("data-code"));
    })

    $("#language-select a").on("click", function () {
        $('#selectLanguageModal').modal('show');

    });

    $("#selectLanguageModal a").on("click", function () {

        var pathname = window.location.pathname; // Returns path only
        var url = window.location.href;
        var host = url.replace(pathname, "");

        //alert(pathname);

        if (pathname.indexOf("/") == 0 && pathname.charAt(3) == "/") {

            var newUrl = host + "/" + pathname.replace(pathname.substr(0, 3), $(this).attr("data-language"));
            //alert(newUrl + " |1");
            window.location = newUrl;

        } else if (pathname.indexOf("/") == 0 && pathname.charAt(3) != "/" && pathname.length > 3) {
            var newUrl = host + "/" + $(this).attr("data-language") + pathname;
            //alert(newUrl + " |2");
            window.location = newUrl;

        } else if (pathname.length <= 3) {
            if (pathname.length > 1) {
                var newUrl = window.location.href.replace(pathname, "") + "/" + $(this).attr("data-language") + "/";
            } else {
                var newUrl = window.location.href + $(this).attr("data-language") + "/";
            }
            //newUrl = newUrl.replace("//", "/");
            //alert(newUrl +" |3"+pathname);
            window.location = newUrl;
        }

    });

    bindEditors();
    if($("#jobs").length){
        jobsClick();
    }
    tidyForms();


    if ($(".accomodationTables").length) {
        transformTables();
    }


    if($("#intensity-caps").length){
        doCaps();
    }

    if($("#courseSelect").length) {
        doCourses();
    }

    if($(".stepContainer").length){
        $(".stepContainer input").on("change", function () {
            var id = $(this).val();
            console.log(id);
            $(".stepsBox .termStep").hide();
            $("[data-step=" + id + "]").show();
        })
    }

});


function doCourses() {
    $("#courseSelect .course:first-child").addClass("active");
    var selectedID = $("#courseSelect .course:first-child").attr("data-course");
    $("#course-details [data-course=" + selectedID + "]").show();

    $("#courseSelect .course").on("click",function () {
        $("#courseSelect .course").removeClass("active");
        $("#course-details [data-course]").hide();
        $(this).addClass("active");
        selectedID = $(this).attr("data-course");
        $("#course-details [data-course=" + selectedID + "]").show();
    })
}


function doCaps() {
    var length = $("#intensity-caps").attr("data-intensity");
    $("#intensity-caps .icon").each(function (i) {
        
        if (length > i) {
            $(this).addClass("selected");
        }
        

    })
}

function tidyForms() {
    $(".fsForm").each(function () {
        $(this).find("input[type=text],input[type=email],input[type=number],select, textarea").addClass("form-control");
        $(this).find("[type=submit]").addClass("btn").addClass("btn-default");
    });
}

    function jobsClick() {

    
        $(".job-detail").html($(".job:first-child").addClass("selected").find(".detail").html());
        $(".job").on("click", function () {
            $(".job").each(function () {
                $(this).removeClass("selected");
            });
            $(".job-detail").html($(this).find(".detail").html());
            $(this).addClass("selected");
            $('html, body').animate({
                scrollTop: $(".job-detail").offset().top + -40
            }, 400);
        });

    

    }


    function transformTables() {
   
        if ($(window).width() < 450) {
        
            $(".accomodationTables table").each(function () {
                var table = $(this);
                table.hide();
                var tempTable1 = $("<table class='table-xs'></table>");
                var tempTable2 = $("<table class='table-xs'></table>");
                var hasTbl2 = false;

                $(table).find("tr").each(function (i) {
                    $(this).find("td").each(function (j) {
                        if (i == 0 && j == 1) {
                       
                            tempTable1.append("<tr><td class='heading'>"+$(this).html()+"</tr></td>");
                        }
                        if (i == 0 && j == 2) {
                            if($(this).html() != ""){
                                hasTbl2 = true;
                                tempTable2.append("<tr><td class='heading'>" + $(this).html() + "</tr></td>");
                            }
                        
                        }

                        if (i != 0 && j == 0) {
                            tempTable1.append("<tr><td class='title'>" + $(this).html() + "</tr></td>");
                            tempTable2.append("<tr><td class='title'>" + $(this).html() + "</tr></td>");
                        }

                        if (i != 0 && j == 1) {
                            tempTable1.append("<tr><td class='content'>" + $(this).html() + "</tr></td>");
                        }
                        if (i != 0 && j == 2) {
                            tempTable2.append("<tr><td class='content'>" + $(this).html() + "</tr></td>");
                        }
                    })
                })
                table.parent().append(tempTable1);
                if(hasTbl2){
                    table.parent().append(tempTable2);
                }
            
            });
        }
    }


    function redirectToReturnURL() {
        var returnURL = $("input[name=returnUrl]").val();
        if (returnURL == "") {
            returnURL = "/";
        }

        window.location = returnURL;
    }


    function redirectToLockout() {
        window.location = "/account/lockout";
    }




    function reloadPage() {

        window.location = window.location.href;
    }

    function displayEditModal(data) {
  
        $('#editContentModal').modal('show');

        $("#editContentBox textarea").val("");
        if (data.data.length) {
            $("#editContentBox input[name=contentBodyID]").val(data.data[0].contentBodyID);
            if (tinyMCE.get('pageContentEditor') != null) {
                tinyMCE.get('pageContentEditor').setContent(data.data[0].body);
            }
            
        }
    
        $('#editContentModal .modal-title').html("Edit content section ");
        //bindEditors();
    
    }


    function displayEditModalNoData(contentID, name, code) {
        console.log("no data modal hit");

        $('#editContentModal').modal('show');
        $('#editContentBox textarea').val('');
       
        $('#editContentBox input[name=contentID]').val(contentID);
        $('#editContentBox input[name=name]').val(name);
        $('#editContentBox input[name=code]').val(code);

        //tinyMCE.get('pageContentEditor').setContent();
        $('#editContentModal .modal-title').html('add new content');

        if (tinyMCE.get('pageContentEditor') != null) {
            tinyMCE.get('pageContentEditor').setContent("");
        }

    }


    function displayAlert(type, message) {
        $("#info-bar").html('<div class="alert alert-' + type + ' alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + type + '</strong> ' + message + '</div>');
        $('html, body').animate({
            scrollTop: $("#info-bar").offset().top + -100
        }, 400);

    }




    function ajaxRequest(method, action, data, callback) {

        $.ajax({
            method: method,
            url: action,
            data: data,
            contentType: false,
            processData: false
        })
          .done(function (data, status, jqXHR) {
              //add dailog stuff

              if (data.statusCode == 1) {
                  if (callback == null) {
                      callback = data.callback;
                  }
                  window[data.callback](data);
              }

              if (data.message != null) {

                  var alertType;

                  if (data.statusCode >= 1) {
                      alertType = "success";
                      alertHeading = "Success"
                  } else {
                      alertType = "warning";
                      alertHeading = "Warning"
                  }

                  if ($("#info-bar").length) {
                      $("#info-bar").html('<div class="alert alert-' + alertType + ' alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + alertHeading + '</strong> ' + data.message + '</div>');
                      $('html, body').animate({
                          scrollTop: $("#info-bar").offset().top + -100
                      }, 400);
                  }

              }

          })
            .fail(function (jqXHR, textStatus) {
                if ($("#info-bar").length) {
                    $("#info-bar").html('<div class="alert alert-warning alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>Warning!</strong> Ajax failed, code is ' + jqXHR.status + '.  Call Batman.</div>');
                    $('html, body').animate({
                        scrollTop: $("#info-bar").offset().top + -100
                    }, 400);
                }

            });


    }


    function getContent(content, name, code) {
        var body = null;

        for (var i = 0; i < content.length; i++) {
            if (content[i].name == name) {
                for (var k = 0; k < content[i].contentCollection.length; k++) {
                    if (content[i].contentCollection[k].code == code) {
                        body = content[i].contentCollection[k].body;
                    }
                }
            }
        }

        return body;
    }

    //makes tinymce plugins work properly after they have been hidden via css.
    $(document).on('focusin', function (e) {
        if ($(e.target).closest(".mce-window").length) {
            e.stopImmediatePropagation();
        }
    });

    function bindEditors() {
        if ($(".editable").length) {
            tinymce.baseURL = "/Scripts/tinymce";
            tinymce.init({
                selector: "[data-wysiwyg]",
                height: "400",
                style_formats: [
            {
                title: "Headers", items: [
                   { title: "Header 1", format: "h1" },
                   { title: "Header 2", format: "h2" },
                   { title: "Header 3", format: "h3" },
                   { title: "Header 4", format: "h4" },
                   { title: "Header 5", format: "h5" },
                   { title: "Header 6", format: "h6" }
                ]
            },
            {
                title: "Inline", items: [
                   { title: "Bold", icon: "bold", format: "bold" },
                   { title: "Italic", icon: "italic", format: "italic" },
                   { title: "Underline", icon: "underline", format: "underline" },
                   { title: "Strikethrough", icon: "strikethrough", format: "strikethrough" },
                   { title: "Superscript", icon: "superscript", format: "superscript" },
                   { title: "Subscript", icon: "subscript", format: "subscript" },
                   { title: "Code", icon: "code", format: "code" }
                ]
            },
            {
                title: "Blocks", items: [
                   { title: "Paragraph", format: "p" },
                   { title: "Blockquote", format: "blockquote" },
                   { title: "Div", format: "div" },
                   { title: "Pre", format: "pre" }
                ]
            },
            {
                title: "Alignment", items: [
                   { title: "Left", icon: "alignleft", format: "alignleft" },
                   { title: "Center", icon: "aligncenter", format: "aligncenter" },
                   { title: "Right", icon: "alignright", format: "alignright" },
                   { title: "Justify", icon: "alignjustify", format: "alignjustify" }
                ]
            },
            {
                title: "Fonts", items: [
                   { title: "Lilita One", inline: "span", styles: { 'font-family': 'Lilita One' } }
                ]
            }
                ],
                theme: "modern",
                valid_elements: "*[*]",
                plugins: [
                    "advlist autolink lists link image charmap print preview hr anchor pagebreak",
                    "searchreplace wordcount visualblocks visualchars  fullscreen",
                    "insertdatetime media nonbreaking save table contextmenu directionality",
                    "template paste textcolor colorpicker textpattern imagetools code"
                ],
                toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
                toolbar2: "print preview media | forecolor backcolor emoticons | code",
                image_advtab: true
            });
        }
    

    }

