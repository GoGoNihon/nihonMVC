
$(document).ready(function () {
    
    bindEditors();

    bindAjaxSubmit();


    $("[data-btn-modal-save]").on("click", function (event) {
        event.preventDefault();
        $(this).closest('.modal').modal('hide');
        $(this).closest('form').submit();
    });

    //admin panel collapse
    doAdminPanels();



    //admin page page select
    $("#pageSelect").on("change", function () {
        ajaxRequest("post", "/api/getPage/" + $("#pageSelect").val(),null, fillPage);
    })

    $("#languageSelect").on("change", function () {
        ajaxRequest("post", "/api/getLanguage/" + $("#languageSelect").val(), null, fillLanguage);
    })

    $("#contentSelect").on("change", function () {
        ajaxRequest("post", "/api/getContentByID/" + $("#contentSelect").val(), null, fillContent);
    })

    $("[data-locationselect]").on("change", function () {
        refreshLocation();
        
    })


    $("[data-updatePage]").off();
    $("[data-updatePage]").on("click", function () {
        var form = new FormData();
        form.append("name", $("#pageInfo [name=name]").val());
        form.append("description", $("#pageInfo [name=description]").val());
        form.append("pageID", $("#pageSelect").val());
        ajaxSubmit("post", "/api/editPage", form, "getPageList");
        
      

    });


    $("[data-deletePage]").off();
    $("[data-deletePage]").on("click", function (e) {
        var id = $("#pageSelect").val();
        e.preventDefault();
        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this page and all of its content?");
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deletePage/" + id, null, "getPageList");
            });
    });

    
    $("[data-deletecontent]").off();
    $("[data-deletecontent]").on("click", function (e) {
        var id = $("#contentSelect").val();
        e.preventDefault();
        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this content area?");
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deleteContent/" + id, null, "getPageList");
            });
    });

    //get page list
    if ($("#pageSelect").length) {
        getPageList();
    }
   
    //get language list
    if ($("#languageSelect").length) {
        getLanguageList();
    }

    if($("[data-userList]").length){
        loadUserList();
    }


    if ($("[data-roleList]").length) {
        loadRoleList();
    }

    if ($("[data-schoollistcourse]").length) {
        ajaxSubmit("post", "/api/getSchoolList/", null, "fillSchoolCourseList");
        $("[data-schoollistcourse]").on("change", function () {
            blankCourse();
            getCourses();
            
        })
    }
    

    /// schoool page stuff

    if ($("[data-schoolList]").length) {
        getLocations();
    }

    $("#school-list").on("change", function () {
        getSchool();
    })

    $("[data-courseselect]").on("change", function () {
        getCourse();
        fetchlengths();
        
    })
    

    ///// courses

    if ($("[data-courseshortselect]").length) {
       
        getShortCourses();

        $("[data-deleteShortCourse]").on("click", function (e) {
            var id = $("[data-courseshortselect]").val();
            e.preventDefault();
            $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this short course?");
            $('#confirmDeleteModal').modal()
                .one('click', '[data-confirmedDelete]', function () {
                    $('#confirmDeleteModal').modal("hide");
                    ajaxSubmit("post", "/api/deleteShortCourse/" + id, null, "getShortCourses");
                });
        });


        $("[data-courseshortselect]").on("change", function () {
            
            fillSchoolsShortList();
            
        });
    }
    
    /////////////////////////  short courses   /////////////////////////////////////

    


});

////////////////////////////  short courses  ///////////////////////////////////////


function doAdminPanels() {

    $(".admin .panel-heading").off();
    $(".admin .panel-heading").on("click", function () {
        $(this).toggleClass("open");
        $(this).closest(".panel").find(".panel-body").eq(0).slideToggle(400);

        if ($(this).hasClass("open")) {
            $(this).find(".icon").html("<i class='icon icon-minus-circled'></i>");
        } else {
            $(this).find(".icon").html("<i class='icon icon-plus-circled'></i>");
        }

    })
}


function getShortCourse() {
    blankShortCourse();
    var id = $("[data-courseshortselect]").val();

    if (id != null && id != 0) {
        ajaxRequest("post", "/api/getShortCourse/" + id, null, fillShortCourse);
    }

}


function fillSchoolsShort(data) {
    $("[data-schoollistshort]").html('<option value="0">select school</option><option disabled>-----</option>');
    for (var i = 0; i < data.data.length; i++) {
        $("[data-schoollistshort]").append("<option value='" + data.data[i].id + "'>" + data.data[i].name + "</option> ");
    }
    getShortCourse();
}

function fillShortCourse(data) {
  
    var courseID = $("[data-courseshortselect]").val();
    $("#editShortCourseGeneral").attr("action", "/api/editShortCourseGeneral/" + courseID + "/getShortCourse");
    $("#addShortCourseFeature").attr("action", "/api/addShortCourseFeature/" + courseID + "/getShortCourse");
    $("#addShortCourseCultureGallery").attr("action", "/api/addShortCourseCulturalGallery/" + courseID + "/getShortCourse");
    $("#addShortCourseFaq").attr("action", "/api/addShortCourseQuestion/" + courseID + "/getShortCourse");
    
    getShortCourseGalleries();

    var name;
    var descriptionHeading;
    var dates;


    for (var i = 0; i < data.data.content.length; i++) {
        if (data.data.content[i].name == "name") {
            for (var j = 0; j < data.data.content[i].contentCollection.length; j++) {
                if (data.data.content[i].contentCollection[j].code == "en") {
                    name = data.data.content[i].contentCollection[j].body;
                }
            }
        } else if (data.data.content[i].name == "descriptionHeading") {
            for (var j = 0; j < data.data.content[i].contentCollection.length; j++) {
                if (data.data.content[i].contentCollection[j].code == "en") {
                    descriptionHeading = data.data.content[i].contentCollection[j].body;
                }
            }
        } else if (data.data.content[i].name == "dates") {
            for (var j = 0; j < data.data.content[i].contentCollection.length; j++) {
                if (data.data.content[i].contentCollection[j].code == "en") {
                    dates = data.data.content[i].contentCollection[j].body;
                }
            }
        }

    }


    $("#editShortCourseGeneral [name=name]").val(name);
    $("#editShortCourseGeneral [name=url]").val(data.data.url);
    $("#editShortCourseGeneral [name=bookNowLink]").val(data.data.bookNowLink);

    $("#editShortCourseGeneral [name=video]").val(data.data.video);
    if (data.data.video != null) {
        $("[data-shortcourseheadingvideo]").html("");
        var youtubePlayerDiv = '<iframe type="text/html" src="http://www.youtube.com/embed/' + data.data.video + '?enablejsapi=1&showinfo=0" frameborder="0"></iframe>';
        $("[data-shortcourseheadingvideo]").append(youtubePlayerDiv);

    } else {
        $("[data-shortcourseheadingvideo]").html("no video");
    }


    $("#editShortCourseGeneral [name=descriptionHeading]").val(descriptionHeading);
    tinymce.EditorManager.get('dates').setContent(dates);

    if (data.data.videoCover != null && data.data.videoCover != "") {
        $(".videoCover").html("");
        $(".videoCover").css("background-image", "url('" + data.data.videoCover + "?" + Math.floor(Math.random() * 100) + "')");
        $(".videoCover").css("height", "150px");
    } else {
        $(".videoCover").html("no image");
        $(".videoCover").css("background-image", "none");
        $(".videoCover").css("height", "auto");
    }


    if (data.data.descriptionImage != null && data.data.descriptionImage != "") {
        $(".descriptionImage").html("");
        $(".descriptionImage").css("background-image", "url('" + data.data.descriptionImage + "?" + Math.floor(Math.random() * 100) + "')");
        $(".descriptionImage").css("height", "150px");
    } else {
        $(".descriptionImage").html("no image");
        $(".descriptionImage").css("background-image", "none");
        $(".descriptionImage").css("height", "auto");
    }


    $("[data-schoollistshort]").val(data.data.schoolID);
        
   
    //features
    //getFeaturesGallery(data.data.featureGalleryID);
    
    //cultural galleries

    getCulturalGalleries(courseID);


    //plans

    $("[data-addPlanFeature]").off();
    $("[data-addPlanFeature]").on("click", function () {

        var form = new FormData();
        form.append("name", $("#addPlanFeature [name=name]").val());
        form.append("bronze", $("#addPlanFeature [name=bronze]").prop("checked"));
        form.append("silver", $("#addPlanFeature [name=silver]").prop("checked"));
        form.append("gold", $("#addPlanFeature [name=gold]").prop("checked"));
        ajaxSubmit("post", "/api/addPlanFeature/" + courseID, form, "displayPlan");

    });

    displayPlan();
    displayShortCourseFaq(data.data.faq);

}






function displayShortCourseFaq(data) {
    $("#FAQContainer *").remove();
    var container = $("#FAQContainer");

    for (var i = 0; i < data.questions.length; i++) {

        var question;
        var answer;

        for (var j = 0; j < data.questions[i].content.length; j++) {

            if (data.questions[i].content[j].name == "question") {
                for (var k = 0; k < data.questions[i].content[j].contentCollection.length; k++) {
                    if (data.questions[i].content[j].contentCollection[k].code == "en") {
                        question = data.questions[i].content[j].contentCollection[k].body;
                    }
                    
                }
            }
            if (data.questions[i].content[j].name == "answer") {
                for (var k = 0; k < data.questions[i].content[j].contentCollection.length; k++) {
                    if (data.questions[i].content[j].contentCollection[k].code == "en") {
                        answer = data.questions[i].content[j].contentCollection[k].body;
                    }

                }
            }
        }

        var html = '<div class="FAQQuestion">\
								<div class="panel panel-default panel-xs">\
									<div class="panel-heading clearfix">\
										<h4 class="panel-title"><i class="icon icon-question-circled"></i> question '+(i+1) +'</h4>\
										<div class="icon">\
											<i class="icon icon-plus-circled"></i>\
										</div>\
									</div>\
									<div class="panel-body">\
										<div class="row">\
											<div class="form-group col-xs-12">\
												<div class="input-group">\
													<div class="input-group-addon black"><i class="icon icon-question-circled"></i>question</div>\
													<input class="form-control input-sm" name="question" value="'+question+'" required>\
												</div>\
											</div>\
										</div>\
										<div class="row">\
											<div class="form-group col-xs-12">\
												<div class="input-group-addon black single"><i class="icon icon-comment"></i>answer</div>\
												<textarea class="form-control" data-wysiwyg-xs name="answer" id="answer'+data.questions[i].id+'">' + answer + '</textarea>\
											</div>\
										</div>\
										<div class="row">\
											<div class="form-group col-xs-12">\
												<div class="input-group input-group-sm">\
													<div style="width:100%"></div>\
													<div class="input-group-addon black "><i class="icon icon-cog"></i></div>\
													<span class="input-group-btn">\
														<button type="button" data-updateFAQQuestion data-id="' + data.questions[i].id + '" class="btn btn-info btn-xs">edit</button>\
														<button type="button" data-deleteFAQQuestion data-id="' + data.questions[i].id + '" class="btn btn-danger btn-xs">delete</button>\
													</span>\
												</div>\
											</div>\
										</div>\
									</div>\
								</div>';

        container.append(html);
    }


    $("[data-updateFAQQuestion]").off();
    $("[data-updateFAQQuestion]").on("click", function () {
        var id = $(this).attr("data-id");
        var form = new FormData();
        form.append("question", $(this).closest(".FAQQuestion").find("[name=question]").val());
        form.append("answer", tinymce.EditorManager.get("answer" + id).getContent());
        ajaxSubmit("post", "/api/editQuestion/" + id, form, "getShortCourse");
    });


    $("[data-deletefaqquestion]").off();
    $("[data-deletefaqquestion]").on("click", function (e) {
        var id = $(this).attr("data-id");
        e.preventDefault();
        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this question?");
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deleteQuestion/" + id, null, "setupPageFAQs");
            });
    });

    doAdminPanels();
    bindEditors();



}



//// plans

function displayPlan(){
    ajaxRequest("post", "/api/getPlan/" + $("[data-courseshortselect]").val(), null, loadPlan);
}

function loadPlan(data){
    var container = $("#editPlanBox");

    for (var i = 0; i < data.data.length; i++) {
        var name;

        var html = '<div class="row planFeature">\
							<div class="form-group col-xs-12">\
								<div class="input-group">\
									<div class="input-group-addon black width30" data-planFeatureName><i class="icon icon-pencil-square-o"></i> </div>\
									<div class="input-group-addon">\
										<div class="checkbox">\
											<label>\
												<input type="checkbox" name="bronze"> bronze\
											</label>\
										</div>\
									</div>\
									<div class="input-group-addon">\
										<div class="checkbox">\
											<label>\
												<input type="checkbox" name="silver"> silver\
											</label>\
										</div>\
									</div>\
									<div class="input-group-addon">\
										<div class="checkbox">\
											<label>\
												<input type="checkbox" name="gold"> gold\
											</label>\
										</div>\
									</div>\
									<span class="input-group-btn">\
										<button data-editPlanFeature class="btn btn-primary btn-sm" type="button">update</button>\
                                        <button data-deletePlanFeature data-id class="btn btn-danger btn-sm" type="button">delete</button>\
									</span>\
								</div>\
							</div>\
						</div>';
        html = $(html);

        for (var j = 0; j < data.data[i].content.length; j++) {
            if (data.data[i].content[j].name == "name") {
                for (var k = 0; k < data.data[i].content[j].contentCollection.length; k++) {
                    if (data.data[i].content[j].contentCollection[k].code = "en") {
                        name = data.data[i].content[j].contentCollection[k].body;
                    }
                }
            }

        }

        html.find("[data-editPlanFeature]").attr("data-id", data.data[i].id);
        html.find("[data-deletePlanFeature]").attr("data-id", data.data[i].id);
        html.find("[name=bronze]").prop("checked", data.data[i].bronze);
        html.find("[name=silver]").prop("checked", data.data[i].silver);
        html.find("[name=gold]").prop("checked", data.data[i].gold);
        html.find("[data-planFeatureName]").append(name);
        container.append(html);
    }

    $("[data-deleteplanfeature]").off();
    $("[data-deleteplanfeature]").on("click", function (e) {
        var id = $(this).attr("data-id");
        e.preventDefault();
        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this plan feature?");
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deletePlanFeature/" + id, null, "getShortCourse");
            });
    });

    $("[data-editPlanFeature]").off();
    $("[data-editPlanFeature]").on("click", function () {
        var id = $(this).attr("data-id");
        var form = new FormData();
        form.append("gold", $(this).closest(".planFeature").find("[name=gold]").prop("checked"));
        form.append("silver", $(this).closest(".planFeature").find("[name=silver]").prop("checked"));
        form.append("bronze", $(this).closest(".planFeature").find("[name=bronze]").prop("checked"));

        ajaxSubmit("post", "/api/editPlanFeature/" + id, form, "getShortCourse");

    });



}


///////////////  galleries  ///////////////////////////

function getShortCourseGalleries() {
    ajaxRequest("post", "/api/getShortCourseGalleries/" + $("[data-courseshortselect]").val(), null, displayShortCourseGalleries);
}


function displayShortCourseGalleries(data) {
    $("#editShortCourseGalleries *").remove();
    var container = $("#editShortCourseGalleries");

    var html = "";
    

    for (var i = 0; i < data.data.length; i++) {
        var name;
        var id;

        for (var j = 0; j < data.data[i].content.length; j++) {
            if (data.data[i].content[j].name == "name") {
                for (var k = 0; k < data.data[i].content[j].contentCollection.length; k++) {
                    if (data.data[i].content[j].contentCollection[k].code = "en") {
                        name = data.data[i].content[j].contentCollection[k].body;
                    }
                }
            }
        }


        html = '<div class="input-group galleryList-row">\
					<div class="input-group-addon black"><i class="icon icon-camera"></i>'+ name + '</div>\
					<div class="input-group-addon" style="width:100%">'+ data.data[i].galleryImages.length + ' images</div>\
					<span class="input-group-btn">\
                        <button class="btn btn-primary btn-sm" type="button" data-id="' + data.data[i].id + '" data-editGallery>edit gallery</button>\
					</span>\
				</div>';
        container.append(html);


        

    }

    $("#editShortCourseGalleries [data-editGallery]").off();
    $("#editShortCourseGalleries [data-editGallery]").on("click", function (e) {
        var id = $(this).attr("data-id");
        e.preventDefault();
        getGalleryFormData(id);
    });



}




//////////  Cultural //////////////////////////////////

function getCulturalGalleries(courseID) {
    ajaxRequest("post", "/api/getCulturalGalleries/" + courseID, null, displayCulturalGalleries);
}


function displayCulturalGalleries(data) {
    $("#editCulturalGalleries *").remove();
    var container = $("#editCulturalGalleries");
    
    var html = "";


    for (var i = 0; i < data.data.length; i++) {
        var name;
        var id;
        for (var j = 0; j < data.data[i].content.length; j++) {
            if (data.data[i].content[j].name == "name") {
                for (var k = 0; k < data.data[i].content[j].contentCollection.length; k++) {
                    if (data.data[i].content[j].contentCollection[k].code = "en") {
                        name = data.data[i].content[j].contentCollection[k].body;
                    }
                }
            }

            html = '<div class="input-group galleryList-row">\
					<div class="input-group-addon black"><i class="icon icon-camera"></i>'+ name + '</div>\
					<div class="input-group-addon" style="width:100%">'+ data.data[i].galleryImages.length + ' images</div>\
					<span class="input-group-btn">\
                        <button class="btn btn-primary btn-sm" type="button" data-id="' + data.data[i].id + '" data-editGallery>edit gallery</button>\
                        <button class="btn btn-danger btn-sm" type="button" data-id="'+data.data[i].id+'" data-deleteGallery>delete gallery</button>\
					</span>\
				</div>';
            container.append(html);
        }

    }


    $("[data-deleteGallery]").on("click", function (e) {
        var id = $(this).attr("data-id");
        e.preventDefault();
        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this gallery?");
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deleteGallery/" + id, null, "getShortCourse");
            });
    });


    $("[data-editGallery]").on("click", function (e) {
        var id = $(this).attr("data-id");
        e.preventDefault();
        getGalleryFormData(id, "getShortCourse");
    });



}


function getGalleryFormData(id) {
    if (id == "" || id == null || isNaN(id)) {
        var id = $("#editGalleryModal").attr("data-galID");
    }

    ajaxRequest("post", "/api/getGallery/" + id, null, loadGalleryForm);
}



function blankContent() {
    $("#addContentName").val(""); 

}

function afterGalleryRefresh() {
   
    if ($("#pageSelect").length) {
    
        getPageContent();
    }

    if ($("#addFaqQuestion").length) {
        $("#addFaqQuestion [name=question]").val("");
        if (tinymce.EditorManager.get("addFaqAnswer") != null) {
            tinymce.EditorManager.get("addFaqAnswer").setContent("");
        }
        
    }

    if ($(".shortCourseAdmin").length) {
        getShortCourse();
    }
    
}


function loadGalleryForm(data) {
    var id = data.data.id;
    $("#galleryPicturesList *").remove();
    container = $("#galleryPicturesList");
    $("#editGalleryEditName").val(data.data.content[0].contentCollection[0].body);

    $("#editGalleryModal").attr("data-galID", data.data.id);
    tinymce.EditorManager.get("addImageText").setContent("");
    $("#addImageTitle").val("");
    $("#addImage").val("");
    $(".galleryImage [name=tag]").val("");

    $("[data-editGalleryEditName]").off();
    $("[data-editGalleryEditName]").one("click", function () {
        if ($("[name-editGalleryEditName]").val() != "") {
            form = new FormData();
            form.append("name", $("#editGalleryEditName").val());
            ajaxSubmit("post", "/api/editGallery/" + id, form, "afterGalleryRefresh");
            $("#editGalleryModal").modal("hide");
        }

    });

    $("[data-updategalleryaddimage]").off();
    $("[data-updategalleryaddimage]").one("click", function () {
        
        var form = new FormData();
        form.append("image", $("#addImage")[0].files[0]);
        form.append("title", $("#addImageTitle").val());
        form.append("tag", $(".galleryImage [name=tag]").val());
        form.append("text", tinymce.EditorManager.get("addImageText").getContent());
        $("#editGalleryModal").modal("hide");
        ajaxSubmit("post", "/api/addGalleryImage/" + id, form, "afterGalleryRefresh");
        
    })


    for (var i = 0; i < data.data.galleryImages.length; i++) {
        var form = $("#galleryImageForm .panel").clone();
        form.off();
        var title;
        var text;

        

        for (var j = 0; j < data.data.galleryImages[i].content.length; j++) {
            form.find("[name=tag]").val(data.data.galleryImages[i].tag);
            form.find("textarea").attr("id","editGalleryImageText"+data.data.galleryImages[i].id);
            if (data.data.galleryImages[i].content[j].name == "title") {
                for (var k = 0; k < data.data.galleryImages[i].content[j].contentCollection.length; k++) {
                    if (data.data.galleryImages[i].content[j].contentCollection[k].code = "en") {
                        title = data.data.galleryImages[i].content[j].contentCollection[k].body;
                        form.find("[name=title]").val(title);
                    }
                }
            }
            if (data.data.galleryImages[i].content[j].name == "text") {
                for (var k = 0; k < data.data.galleryImages[i].content[j].contentCollection.length; k++) {
                    if (data.data.galleryImages[i].content[j].contentCollection[k].code = "en") {
                        text = data.data.galleryImages[i].content[j].contentCollection[k].body;
                      
                        form.find("#editGalleryImageText" + data.data.galleryImages[i].id).html(text);
                        form.find("#editGalleryImageText" + data.data.galleryImages[i].id).attr("data-wysiwyg-xs","");
                        //tinymce.EditorManager.get("editGalleryImageText" + data.data.galleryImages[i].id).setContent(text);
                    }
                }
            }

            
            form.find("#imageHolder").html("<img class='img-responsive' src='"+data.data.galleryImages[i].image+ "?" + Math.floor(Math.random() * 100) +"' >");
            form.find("[data-updategalleryimage]").attr("data-id", data.data.galleryImages[i].id);
            form.find("[data-deletegalleryimage]").attr("data-id", data.data.galleryImages[i].id);
            container.append(form);

        }
        

    }
    bindEditors();
    doAdminPanels();

    $('#editGalleryModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deleteShortCourseFeature/" + id, null, "getShortCourse");
            });


    $("[data-updategalleryimage]").off();
    $("[data-updategalleryimage]").on("click", function () {
        var form = new FormData();
        form.append("title", $(this).closest(".galleryImage").find("[name=title]").val());
        form.append("text", tinymce.EditorManager.get("editGalleryImageText" + $(this).attr("data-id")).getContent());
        form.append("galleryImage", $(this).closest(".galleryImage").find("[name=galleryImage]")[0].files[0]);
        form.append("tag", $(this).closest(".galleryImage").find("[name=tag]").val());
        ajaxSubmit("post", "/api/editGalleryImage/" + $(this).attr("data-id"), form, "getShortCourse");
        $("#editGalleryModal").modal("hide");
    });


    $("[data-deletegalleryimage]").off();
    $("[data-deletegalleryimage]").on("click", function (e) {
        var id = $(this).attr("data-id");
        e.preventDefault();

        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this image?");
        $("#editGalleryModal").modal("hide");
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deleteGalleryImage/" + id, null, "afterGalleryRefresh");
            }).one("click", "[data-cancelDelete]", function () {
                $("#editGalleryModal").modal();
            });

    });

}

//features ///////////////////////////////////////////////////////////////////
function getFeaturesGallery(galID) {
    ajaxRequest("post", "/api/getGallery/" + galID, null, displayFeatures);
}

function displayFeatures(data) {

    var container = $("#editFeaturesGallery");
    var title;
    var text;
    var html ="";
    for (var i = 0; i < data.data.galleryImages.length; i++) {

        for (var j = 0; j < data.data.galleryImages[i].content.length; j++) {
            if(data.data.galleryImages[i].content[j].name == "title"){
                for (var k = 0; k < data.data.galleryImages[i].content[j].contentCollection.length; k++) {
                    if (data.data.galleryImages[i].content[j].contentCollection[k].code = "en") {
                        title = data.data.galleryImages[i].content[j].contentCollection[k].body;
                    }
                }
            }
            if (data.data.galleryImages[i].content[j].name == "text") {
                for (var k = 0; k < data.data.galleryImages[i].content[j].contentCollection.length; k++) {
                    if (data.data.galleryImages[i].content[j].contentCollection[k].code = "en") {
                        text = data.data.galleryImages[i].content[j].contentCollection[k].body;
                    }
                }
            }

            

        }

        html = "<div class='item' data-id='" + data.data.galleryImages[i].id + "'>\
                        <div class='form-group col-xs-12 col-sm-6'>\
							<label>image</label>\
							<input type='file' class='form-control input-sm' name='galleryImage' accept='image/x-png, image/gif, image/jpeg'>\
						</div>\
                        <div class='form-group col-xs-12 col-sm-6'>\
                        <img class='img-responsive' style='max-height:200px;' src='" + data.data.galleryImages[i].image + "?" + Math.floor(Math.random() * 100) + "'>\
						</div>\
                        <div class='row'><div class='form-group col-xs-12'><label>heading</label><input class='form-control' type='text' name='heading' value='" + title + "'></div></div>\
                        <div class='row'><div class='form-group col-xs-12'><label>body</label><textarea class='form-control' data-wysiwyg-xs name='text" + data.data.galleryImages[i].id + "' id='editFeatureBody" + data.data.galleryImages[i].id + "'></textarea></div></div>\
                        <div class='row'><div class='form-group col-xs-12 text-right'>\
                        <button type='button' data-updateShortCourseFeature class='btn btn-info btn-xs'>update</button> \
                        <button type='button' data-deleteShortCourseFeature class='btn btn-danger btn-xs'>delete</button></div></div>\
                        </div>";
        container.append(html);
    }
    
    bindEditors();
    bindFeatureGalleryControls();

    $("#editFeaturesGallery .item").each(function () {

        for (var i = 0; i < data.data.galleryImages.length; i++) {
            if ($(this).attr("data-id") == data.data.galleryImages[i].id) {

                for (var j = 0; j < data.data.galleryImages[i].content.length; j++) {
                    
                    if (data.data.galleryImages[i].content[j].name == "text") {
                        for (var k = 0; k < data.data.galleryImages[i].content[j].contentCollection.length; k++) {
                            if (data.data.galleryImages[i].content[j].contentCollection[k].code = "en") {
                                tinymce.EditorManager.get("editFeatureBody" + $(this).attr("data-id")).setContent(data.data.galleryImages[i].content[j].contentCollection[k].body);
                            }
                        }
                    }
                }
            }
        }

    });
    
    
}



function bindFeatureGalleryControls() {
    
    $("[data-updateShortCourseFeature]").on("click", function () {
        var id = $(this).closest(".item").attr("data-id");
        var heading = $(this).closest(".item").find("[name=heading]").val();
        var galleryImage = $(this).closest(".item").find("[name=galleryImage]")[0].files[0];
        var body = tinymce.EditorManager.get("editFeatureBody" + id).getContent();

        form = new FormData();
        form.append("heading", heading);
        form.append("text", body)
        form.append("galleryImage", galleryImage)
        ajaxSubmit("post", "/api/editShortCourseFeature/" + id, form, "getShortCourse");
    });

    $("[data-deleteshortcoursefeature]").on("click", function (e) {
        var id = $(this).closest(".item").attr("data-id");


        e.preventDefault();
        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this feature?");
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deleteShortCourseFeature/" + id, null, "getShortCourse");
            });
    });


    

}

function fillSchoolsShortList() {
    ajaxSubmit("post", "/api/getSchoolList/", null, "fillSchoolsShort");
}

function blankShortCourse() {
    
    $("#editShortCourseGeneral [name=name]").val("");
    $("#editShortCourseGeneral [name=url]").val("");
    $("#editShortCourseGeneral [name=video]").val("");
    $("#editShortCourseGeneral [name=descriptionHeading]").val("");
    if(tinymce.EditorManager.get('dates') != null){
    tinymce.EditorManager.get('dates').setContent("");
    }
    

    $("#editShortCourseGeneral [name=bookNowLink]").val("");

    $(".descriptionImage").html("no image");
    $(".descriptionImage").css("background-image", "none");
    $(".descriptionImage").css("height", "auto");

    $(".videoCover").html("no image");
    $(".videoCover").css("background-image", "none");
    $(".videoCover").css("height", "auto");

    $("[data-shortcourseheadingvideo]").html("no video");

    $("#editFeaturesGallery *").remove();
    $("#editCulturalGalleries *").remove();
    $("#editPlanBox *").remove();
    $("#editShortCourseGalleries *").remove();
    $("#FAQContainer *").remove();

    $("#addShortCourseCultureGallery [name=name]").val("");

    $("#addShortCourseFaq [name=question]").val("");
    tinymce.EditorManager.get('addFaqAnswer').setContent("");

}


function getShortCourses() {

    ajaxRequest("post", "/api/getShortCourses/", null, fillShortCourseList);
}


function fillShortCourseList(data) {

    var select = $("[data-courseshortselect]");
    select.html('<option value="0">select short course</option><option disabled>-----</option>')

    for (var k = 0; k < data.data.length; k++) {
        select.append("<option value='" + data.data[k].id + "'>" + data.data[k].url + "</option>");
    }

}


////////////////////////////  courses  ///////////////////////////////////////


function fetchlengths() {

    if ($("[data-fillLengthsBox]").length && $("[data-courseselect]").val() != 0) {
        ajaxRequest("post", "/api/getCourseLengths/" + $("[data-courseselect]").val(), null, doCourseLengths);
    }
}


function doCourseLengths(data) {
    var content;
    $("#lengths-box").html("");
    for (var i = 0; i < data.data.length; i++) {

        content = '<div class="row"><div class="form-group col-xs-12" ><div class="input-group"><div class="input-group-addon black" style="width:30%">course length is</div><div class="input-group-addon" style="width:30%">';
        if (data.data[i].from != data.data[i].to) {
            content += 'from ' + data.data[i].from + ' to ' + data.data[i].to + ' months';
        } else {
            content += data.data[i].to + ' months';
        }
        content += '</div><div class="input-group-addon black" style="width:30%">';

        if (data.data[i].withVisa) {
            content += ' with visa';
        } else {
            content += ' without visa';
        }

        content += '</div><span class="input-group-btn"><button class="btn btn-danger btn-sm" data-id=' + data.data[i].id + ' data-deleteLength type="button">delete</button></span></div></div></div>';

        $("#lengths-box").append(content);
    }
    bindDeleteLength();
}



function getCourse() {
    blankCourse();
    if ($("[data-courseselect]").val() != 0 && $("[data-courseselect]").val() != null) {
      
        ajaxRequest("post", "/api/getcourse/" + $("[data-courseselect]").val(), null, fillCourseContent);
    }
}


function blankCourse() {
    $("#lengths-box, #stepsBox").html("");
    $(".introductionImage").html("no image");
    $(".introductionImage").css("background-image", "none");
    $(".introductionImage").css("height", "auto");

    $(".demographyImage").html("no image");
    $(".demographyImage").css("background-image", "none");
    $(".demographyImage").css("height", "auto");

    $("#editCourseGeneral [name=name]").val("");
    if (tinymce.EditorManager.get('editCourseIntroduction') != null) {
        tinymce.EditorManager.get('editCourseIntroduction').setContent("");
    }
    
    tinymce.EditorManager.get('costsText').setContent("");
    $("#editCourseSchedule").attr("action", "")
    $("#editCourseSchedule [name=hoursPerWeek]").val(0);
    tinymce.EditorManager.get('scheduleText').setContent("");

    tinymce.EditorManager.get('termStarts').setContent("");

    $("[name='features.jlptClasses']").prop("checked", false);
    $("[name='features.conversationClasses']").prop("checked", false);
    $("[name='features.openClasses']").prop("checked", false);
    $("[name='features.higherEducationPrep']").prop("checked", false);
    $("[name='features.businessClasses']").prop("checked", false);
    $("[name='features.cultureClasses']").prop("checked", false);
    $("[name='features.kansaiStudies']").prop("checked", false);
    $("[name='features.currentEventsClasses']").prop("checked", false);
    $("[name='features.movieClasses']").prop("checked", false);

}


function fillCourseContent(data) {

    $("form#editCourseGeneral").attr("action", "/api/editCourseGeneral/" + data.data.id+"/en/getCourses");
    $("#addCourseLength").attr("action", "/api/addCourseLength/" + data.data.id + "/fetchlengths");
    $("#formTermStarts").attr("action", "/api/editTermStarts/" + data.data.id);
    $("#editCourseFeatures").attr("action", "/api/editCourseFeatures/" + data.data.id);
    

    var name;
    var courseIntroduction;
    var introductionImage;
    var scheduleText;
    var termStarts;
    var costsText;

    introductionImage = data.data.introductionImage;


    name = getContent(data.data.courseContent, "name", "en");
    courseIntroduction = getContent(data.data.courseContent, "courseIntroduction", "en");
    scheduleText = getContent(data.data.courseContent, "scheduleText", "en");
    termStarts = getContent(data.data.courseContent, "termStarts", "en");
    costsText = getContent(data.data.courseContent, "costsText", "en");



    //general
    $("#editCourseGeneral [name=name]").val(name);
    $("#editCourseGeneral [name=type]").val(data.data.type);
    tinymce.EditorManager.get('editCourseIntroduction').setContent(courseIntroduction);
    if (data.data.introductionImage != null && data.data.introductionImage != "") {
        $(".introductionImage").html("");
        $(".introductionImage").css("background-image", "url('" + data.data.introductionImage + "?" + Math.floor(Math.random() * 100) + "')");
        $(".introductionImage").css("height", "150px");
    } else {
        $(".introductionImage").html("no image");
        $(".introductionImage").css("background-image", "none");
        $(".introductionImage").css("height", "auto");
    }


    //schedule

    $("#editCourseSchedule").attr("action", "/api/editSchedule/"+data.data.id+"/en/getCourse")
    $("#editCourseSchedule [name=hoursPerWeek]").val(data.data.hoursPerWeek);
    tinymce.EditorManager.get('scheduleText').setContent("");
    tinymce.EditorManager.get('scheduleText').setContent(scheduleText);


    //term steps 

    tinymce.EditorManager.get('termStarts').setContent(termStarts);

    $("#addTermStep").attr("action", "/api/addTermStep/" + data.data.id + "/en/getCourse");
    tinymce.EditorManager.get('addTermBody').setContent("");
    getTermSteps();

    //features
    $("[name='features.jlptClasses']").prop("checked", data.data.features.jlptClasses);
    $("[name='features.conversationClasses']").prop("checked", data.data.features.conversationClasses);
    $("[name='features.openClasses']").prop("checked", data.data.features.openClasses);
    $("[name='features.higherEducationPrep']").prop("checked", data.data.features.higherEducationPrep);
    $("[name='features.businessClasses']").prop("checked", data.data.features.businessClasses);
    $("[name='features.cultureClasses']").prop("checked", data.data.features.cultureClasses);
    $("[name='features.kansaiStudies']").prop("checked", data.data.features.kansaiStudies);
    $("[name='features.currentEventsClasses']").prop("checked", data.data.features.currentEventsClasses);
    $("[name='features.movieClasses']").prop("checked", data.data.features.movieClasses);

    //costs
    $("#formEditCosts").attr("action", "/api/editCosts/" + data.data.id);
    tinymce.EditorManager.get('costsText').setContent(costsText);

    //demography
    $("#demographyImageForm").attr("action", "/api/addDemographyImage/" + data.data.id + "/getCourse");

    if (data.data.demographyImage != null && data.data.demographyImage != "") {
        $(".demographyImage").html("");
        $(".demographyImage").css("background-image", "url('" + data.data.demographyImage + "?" + Math.floor(Math.random() * 100) + "')");
        $(".demographyImage").css("height", "150px");
    } else {
        $(".demographyImage").html("no image");
        $(".demographyImage").css("background-image", "none");
        $(".demographyImage").css("height", "auto");
    }

    $("#formAddDemography").attr("action", "/api/addDemography/" + data.data.id + "/getDemographics");
    getDemographics();
    


}


function getDemographics(){
    ajaxRequest("post", "/api/getDemographics/" + $("[data-courseselect]").val(), null, doDemographics);
}


//demos
function doDemographics(data) {

    $("#demoBox").html("");
    var total=0;
    for (var i = 0; i < data.data.length; i++) {

        var name = data.data[i].content[0].contentCollection[0].body;
        var percent = data.data[i].percent;
        total = total + percent;
        var id = data.data[i].id;
        var html = '<div class="form-group col-xs-12">\
									<div class="input-group">\
										<div class="input-group-addon">'+name+'</div>\
                                        <div class="input-group-addon"><div style="width:' + percent + '%; background-color:#333; color:#eee;">' + percent + '%</div></div>\
										<span class="input-group-btn">\
											<button class="btn btn-danger btn-sm" data-deleteDemo data-id="' + id + '">delete</button>\
										</span>\
									</div>\
								</div>';
        $("#demoBox").append(html);
        
        
    }
    
    var html = '<div class="form-group col-xs-12">\
									<div class="input-group">\
										<div class="input-group-addon black"><i class="icon icon-percent"></i> total</div>\
                                        <div class="input-group-addon">' + total + '%</div>\
									</div>\
								</div>';
    $("#demoBox").append(html);
    bindDemoControls();
}


function bindDemoControls(){

    $("[data-deleteDemo]").one("click", function (e) {
        var id = $(this).attr("data-id");
        e.preventDefault();
        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this demography?");
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deleteDemography/" + id, null, "getDemographics");
            });
    });

}

//terms
function getTermSteps() {
    ajaxRequest("post", "/api/getTermSteps/" + $("[data-courseselect]").val(), null, doSteps);
}


function doSteps(data) {

    $('#editTermSteps #stepsBox').html("");
    var controlButtons = '<div class="form-group col-xs-12 text-right"><button data-editStep class="btn btn-info btn-xs">edit</button><button data-deleteStep class="btn btn-danger btn-xs">delete</button></div>';

    for (var i = 0; i < data.data.length; i++) {
        var element = $("<div class='item clearfix'><div class='input-group'><div class='input-group-addon black' style='width:30%'>step</div><input type='number' data-stepIndex data-id='" + data.data[i].id + "' data-oldIndex=" + data.data[i].index + " min=1 class='form-control input-sm' name='index' value=" + data.data[i].index + " required></div><div class='body'>" + data.data[i].stepContent[0].contentCollection[0].body + "</div></div>");
        element.append(controlButtons);
        $('#editTermSteps #stepsBox').append(element);
    }

    stepsOnChange(controlButtons);
    bindStepsControls();
    bindUpdateStepOrder();

  
}



function bindUpdateStepOrder() {

    
    
    $("[data-updateStepOrder]").one("click", function () {
        var steps = [];
        $("#stepsBox .item").each(function () {
            
            var step = { index: $(this).find("[data-stepindex]").val(), id: $(this).find("[data-stepindex]").attr("data-id") }
            steps.push(step);
        })

        form = new FormData();
        
        form.append("steps", JSON.stringify(steps));
        console.log(steps);
        //var data = "steps:" + ;
        if (steps.length) {
            ajaxSubmit("post", "/api/updateStepsOrder/", form, "getCourse");
        }
    })

    
   
    
    

}

    function bindStepsControls() {


        $("[data-deleteStep]").one("click", function (e) {
            var stepID = $(this).closest(".item").find("[data-id]").attr("data-id");
            var courseID = $("[data-courseselect]").val();
            e.preventDefault();
            $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this step?");
            $('#confirmDeleteModal').modal()
                .one('click', '[data-confirmedDelete]', function () {
                    $('#confirmDeleteModal').modal("hide");
                    ajaxSubmit("post", "/api/deleteStep/" + stepID + "/"+courseID, null, "getCourse");
                });
        });

        $("[data-editstep]").on("click", function (e) {
            var stepID = $(this).closest(".item").find("[data-id]").attr("data-id");
        
            ajaxSubmit("post", "/api/getStep/" + stepID, null, "showEditStepModal");
        })
    }


    function showEditStepModal(data) {


        $("#editStepModal").modal();
        $("#editStepForm").attr("action", "/api/editTermStep/" + data.data.id + "/en/getCourse");
        tinymce.EditorManager.get('editTermBreakdownBody').setContent(data.data.stepContent[0].contentCollection[0].body);
    }

    function stepsOnChange(controlButtons) {

        $("[data-stepIndex]").on("change", function () {
            var currentID = $(this).attr("data-id");
            var oldIndex = $(this).attr("data-oldIndex")
            var newIndex = $(this).val();
            var steps = [];

            if (newIndex != oldIndex) {
                $("#editTermSteps #stepsBox .item input[type=number]").each(function () {

                    var thisIndex = $(this).val();
                    var thisID = $(this).attr("data-id");
                    var thisBody = $(this).closest(".item").find(".body").html();
                
                    if (currentID != thisID) {

                        if (thisIndex >= newIndex && thisIndex < oldIndex) {
                            $(this).val(++thisIndex);
                        }
                        if (thisIndex <= newIndex && thisIndex > oldIndex) {
                            $(this).val(--thisIndex);


                        }
                    }
                
                    var step = { index: $(this).val(), html: "<div class='item clearfix'><div class='input-group'><div class='input-group-addon black' style='width:30%'>step</div><input type='number' data-stepIndex data-id='" + thisID + "' data-oldIndex=" + $(this).val() + " min=1 class='form-control input-sm' name='index' value=" + $(this).val() + " required></div><div class='body'>" + thisBody + "</div>"+controlButtons+"</div>" }
                    steps.push(step);


                })

                $('#editTermSteps #stepsBox').html("");
                steps.sort(function (a, b) {
                    var keyA = a.index,
                        keyB = b.index;
                    if (keyA < keyB) return -1;
                    if (keyA > keyB) return 1;
                    return 0;
                });
                for (var i = 0; i < steps.length; i++) {
                    $('#editTermSteps #stepsBox').append(steps[i].html);
                }

            }
            bindStepsControls();
            stepsOnChange();



        })
    }


    function bindAjaxSubmit() {

        //bind forms with data-ajax to sbumit via the ajax function
        $("[data-ajax]").on("submit", function (event) {
            event.preventDefault();

            for (var i = 0; i < tinymce.editors.length; i++) {
                tinymce.editors[i].save();
            }


            var form = this;

            var method = $(form).attr("method");
            var action = $(form).attr("action");

            var formData = new FormData($(this)[0]);



            //if ($(this).has("[data-wysiwyg]").length || $(this).has("[data-wysiwyg-sm]").length) {
            //    formData.append("body", tinyMCE.activeEditor.getContent());
            //}


            ajaxSubmit(method, action, formData);

        })

    }


    function refreshLocation() {
        if ($("[data-locationselect]").val() != "0" && $("[data-locationselect]").val() != null && $("[data-locationselect]").val() != "undefined") {
            ajaxRequest("post", "/api/getLocation/" + $("[data-locationselect]").val() + "/en", null, loadLocation);
        } else {
        
            $("[data-locationselect]").val(0);
            $("#editSchoolLocation .image").html("no image");
            $("#editSchoolLocation .image").css("background-image", "none");
            $("#editSchoolLocation .image").css("height", "auto");
    

            $("#editSchoolLocation [name=name]").val("");
            tinymce.EditorManager.get('description').setContent("");
        }

    
    }


    function loadUserList() {
        ajaxRequest("post", "/api/getUsers/", null, fillUsersList);
    
    }

    function fillUsersList(data) {
        var userLIst = $("[data-userList]");
        userLIst.html("");
        var languages = data.data.languages;


        for (var i = 0; i < data.data.users.length; i++) {
        
            var roles = data.data.users[i].Roles;
        
            var roleHtml = "";
            var roleList = $("<select class='roleSelector form-control input-sm'></select>");
            var languageList = $("<select class='languageSelector form-control input-sm'></select>");


            var flagsHtml = $("<div class='flags-list'></div>");
            //build active languages list
            for (var k = 0; k < data.data.userLanguages.length; k++) {
                if (data.data.userLanguages[k].userID == data.data.users[i].Id) {
                    var flag = "";
                    for (var j = 0; j < data.data.languages.length; j++) {
                        if (data.data.languages[j].code == data.data.userLanguages[k].code) {
                            flag = data.data.languages[j].flag;
                        }
                    }

                    flagsHtml.append("<div class='flag-icon'><img class='img-responsive' src='"+flag+"' /></div>");
                }
            
            }


            //build roles list
            for (var k = 0; k < data.data.roles.length; k++) {
                roleList.append("<option value='" + data.data.roles[k].Id + "'>" + data.data.roles[k].Name + "</option>");
            }


            //build language list
            for (var k = 0; k < data.data.languages.length; k++) {
                languageList.append("<option value='" + data.data.languages[k].code + "'>" + data.data.languages[k].name + "</option>");
            }


            for (var j = 0; j < roles.length; j++) {

                for (var k = 0; k < data.data.roles.length; k++) {

                    //fill in role name for users list
                    if(data.data.roles[k].Id == roles[j].RoleId){
                        roleHtml += "<span class='label label-default'>" + data.data.roles[k].Name + "</span>";
                    }
                }
            
            }


            var html=$('<div class="form-group userRow">\
							<div class="input-group">\
								<div class="input-group-addon black" id="username"><i class="icon icon-user"></i>' + data.data.users[i].UserName + '</div>\
								<div class="input-group-addon">privileges</div>\
								<div class="input-group-addon roles">'+roleHtml+flagsHtml[0].outerHTML+'</div>\
								<span class="input-group-btn">\
									<button data-manageUser class="btn btn-primary btn-sm">manage</button>\
									<button data-deleteUser  data-userID="' + data.data.users[i].Id + '" class="btn btn-danger btn-sm">delete</button>\
								</span>\
							</div>\
                            <div class="userManagementBox">\
							<div class="form-group">\
								<div class="input-group">\
									<div class="input-group-addon black"><i class="icon icon-list"></i> role</div>\
									' + roleList[0].outerHTML + '\
									<span class="input-group-btn">\
										<button data-btnAssignRole data-userID="' + data.data.users[i].Id + '" class="btn btn-info btn-sm">assign</button>\
										<button data-userID="' + data.data.users[i].Id + '" data-removeRole class="btn btn-danger btn-sm" type="button">remove</button>\
									</span>\
								</div>\
							</div>\
							<div class="form-group">\
								<div class="input-group">\
									<div class="input-group-addon black"><i class="icon icon-list"></i> language</div>\
									' + languageList[0].outerHTML + '\
									<span class="input-group-btn">\
										<button data-btnAssignLanguage data-userID="' + data.data.users[i].Id + '" class="btn btn-info btn-sm">assign</button>\
										<button data-userID="' + data.data.users[i].Id + '" data-removeLanguage class="btn btn-danger btn-sm" data-deletecourse="" type="button">remove</button>\
									</span>\
								</div>\
							</div>\
                        </div>\
						</div>');


            userLIst.append(html);

        }

        manageUserSetup();

    }




    //schools ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    function getSchoolList() {
        var formData = new FormData();
        ajaxSubmit("post", "/api/getSchoolList/", formData, "fillSchoolList");

    }
    function fillSchoolList(data) {
        $("[data-schoolList]").html("");
        for (var i = 0; i < data.data.length; i++) {
            $("[data-schoolList]").append("<option value='" + data.data[i].id + "'>" + data.data[i].name + "</option> ");
        }
    
        getSchool();

    }


    function fillSchoolCourseList(data) {
        $("[data-schoollistcourse]").html("");
        for (var i = 0; i < data.data.length; i++) {
            $("[data-schoollistcourse]").append("<option value='" + data.data[i].id + "'>" + data.data[i].name + "</option> ");
        }
        getCourses();

    }

    $("[data-deleteSchool]").on("click", function (e) {
        var schoolID = $("[data-schoollist]").val();
        var formData = new FormData();

        e.preventDefault();
        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this school?");
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deleteSchool/" + schoolID, formData, "getSchoolList");
            });
    });


    function bindDeleteLength() {

        $("[data-deleteLength]").on("click", function (e) {
            var courseID = $(this).attr("data-id");


            e.preventDefault();
            $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this length?");
            $('#confirmDeleteModal').modal()
                .one('click', '[data-confirmedDelete]', function () {
                    $('#confirmDeleteModal').modal("hide");
                    ajaxSubmit("post", "/api/deleteLength/" + courseID, null, "fetchlengths");
                });
        });
    }



    $("[data-deleteCourse]").on("click", function (e) {
        var courseID = $("[data-courseselect]").val();

        e.preventDefault();
        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this course?"); 
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deleteCourse/" + courseID, null, "getCourses");
            });
    });

    $("[data-deleteLocation]").on("click", function (e) {
        var locationID = $("[data-locationselect]").val();

        e.preventDefault();
        $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this location?  Other schools might be using it");
        $('#confirmDeleteModal').modal()
            .one('click', '[data-confirmedDelete]', function () {
                $('#confirmDeleteModal').modal("hide");
                ajaxSubmit("post", "/api/deleteLocation/" + locationID, null, "getLocations");
            });
    });





    //function setupSchoolCourseData(data) {


    //    //get all multi language content
    //    var name;
    //    var introduction;
    //    var coursesHeading;

    //    name = getContent(data.data.content, "name", "en");
    //    introduction = getContent(data.data.content, "introduction", "en");
    //    coursesHeading = getContent(data.data.content, "coursesHeading", "en");
   
       
    
    //    $("#editCourseGeneral").attr("action", "/api/addCourseGeneral/" + data.data.id + "/getSchoolList")

    //    $("[name=coursesHeading]").val(coursesHeading);

    //    if (data.data.coursesImage != null && data.data.coursesImage != "") {
    //        $("#editCourseGeneral .image").html("");
    //        $("#editCourseGeneral .image").css("background-image", "url('" + data.data.coursesImage + "?" + Math.floor(Math.random() * 100) + "')");
    //        $("#editCourseGeneral .image").css("height", "150px");
    //    } else {
    //        $("#editCourseGeneral .image").html("no image");
    //        $("#editCourseGeneral .image").css("background-image", "none");
    //        $("#editCourseGeneral .image").css("height", "auto");
    //    }

    //    $("#addCourseForm").attr("action", "/api/addCourse/" + data.data.id + "/getSchoolList");

    //}



    function getSchool() {
        var formData = new FormData();

        if ($("[data-schoolList]").val() != null) {
            ajaxSubmit("post", "/api/getSchool/" + $("[data-schoolList]").val(), formData, "setupSchoolData");
        }
    
    }


    function setupSchoolData(data) {
        //general section
        $("#edit-general").attr("action", "/api/editSchoolGeneral/getSchoolList");
        $("#edit-school-intro").attr("action", "/api/editSchoolIntroIntensity/" + data.data.id + "/getSchoolList");
        $("#edit-school-features").attr("action", "/api/editSchoolFeatures/" + data.data.id + "/getSchoolList");
        $("#addLocationForm").attr("action", "/api/addLocation/getLocations");
        $("#addSchoolStation").attr("action", "/api/addStation/" + data.data.id + "/getSchoolList");
    
        $("#edit-general [name=id]").val(data.data.id);
   

        //get all multi language content
        var name;
        var introduction;
        var coursesHeading;

        name = getContent(data.data.content, "name", "en");
        introduction = getContent(data.data.content, "introduction", "en");
        coursesHeading = getContent(data.data.content, "coursesHeading", "en");

    
    
        $("#edit-general [name=name]").val(name);

    
        $("#edit-general [name=googleMap]").val(data.data.googleMap);
        $("#edit-general option[value=" + data.data.type + "]").prop('selected', true);
        $("#edit-general [name=previewVideo]").val(data.data.previewVideo);
        $("#edit-general [name=url]").val(data.data.url);

        if (data.data.previewVideo != null) {
            $("#edit-general [data-preview-video-container]").html("");
            var youtubePlayerDiv = '<iframe type="text/html" src="http://www.youtube.com/embed/' + data.data.previewVideo + '?enablejsapi=1&showinfo=0" frameborder="0"></iframe>';
            $("#edit-general [data-preview-video-container]").append(youtubePlayerDiv);
        
        } else {
            $("#edit-general [data-preview-video-container]").html("no video");
        }

        $("#edit-general [name=video]").val(data.data.video);

        if (data.data.video != null) {
            $("#edit-general [data-video-container]").html("");
            var youtubePlayerDiv = '<iframe type="text/html" src="http://www.youtube.com/embed/' + data.data.video + '?enablejsapi=1&showinfo=0" frameborder="0"></iframe>';
            $("#edit-general [data-video-container]").append(youtubePlayerDiv);
        
        } else {
            $("#edit-general [data-video-container]").html("no video");
        }
    
        if (data.data.videoCover != null && data.data.videoCover != "") {
            $("#edit-general #generalVideoCoverHolder .image").html("");
            $("#edit-general #generalVideoCoverHolder .image").css("background-image", "url('" + data.data.videoCover + "?" + Math.floor(Math.random() * 100) + "')");
            $("#edit-general #generalVideoCoverHolder .image").css("height", "150px");
        } else {
            $("#edit-general #generalVideoCoverHolder .image").html("no image");
            $("#edit-general #generalVideoCoverHolder .image").css("background-image", "none");
            $("#edit-general #generalVideoCoverHolder .image").css("height", "auto");
        }



        ///intro and intensity section

        if (data.data.introductionImage != null && data.data.introductionImage != "") {
            $("#introImageHolder .image").html("");
            $("#introImageHolder .image").css("background-image", "url('" + data.data.introductionImage + "?" + Math.floor(Math.random() * 100) + "')");
            $("#introImageHolder .image").css("height", "150px");
        } else {
            $("#introImageHolder .image").html("no image");
            $("#introImageHolder .image").css("background-image", "none");
            $("#introImageHolder .image").css("height", "auto");
        }


        if (tinymce.EditorManager.get('address') == null) {
            alert("warning, tinyMCE is stupid, please reload the page");
        } else {
            if (data.data.address == null) {

                tinymce.EditorManager.get('address').setContent("");

            } else {
                tinymce.EditorManager.get('address').setContent(data.data.address);
            }

        }

 
        if (tinymce.EditorManager.get('introduction') == null) {
            alert("warning, tinyMCE is stupid, please reload the page");
        } else {
            if (introduction == null) {

                tinymce.EditorManager.get('introduction').setContent("");

            } else {
                tinymce.EditorManager.get('introduction').setContent(introduction);
            }

        }

 
        $("#edit-school-intro input[name=intensity]").val(data.data.intensity);

        if (data.data.intensityImage != null && data.data.intensityImage != "") {
            $("#intensityImageHolder .image").html("");
            $("#intensityImageHolder .image").css("background-image", "url('" + data.data.intensityImage + "?" + Math.floor(Math.random()*100) + "')");
            $("#intensityImageHolder .image").css("height", "150px");
        } else {
            $("#intensityImageHolder .image").html("no image");
            $("#intensityImageHolder .image").css("background-image", "none");
            $("#intensityImageHolder .image").css("height", "auto");
        }



        //school features section
        if (data.data.featuresImage != null && data.data.featuresImage != "") {
            $("#featuresImageHolder .image").html("");
            $("#featuresImageHolder .image").css("background-image", "url('" + data.data.featuresImage + "?" + Math.floor(Math.random() * 100) + "')");
            $("#featuresImageHolder .image").css("height", "150px");
        } else {
            $("#featuresImageHolder .image").html("no image");
            $("#featuresImageHolder .image").css("background-image", "none");
            $("#featuresImageHolder .image").css("height", "auto");
        }

        $("#edit-school-features [name='features.studentCafe']").prop("checked", data.data.features.studentCafe);
        $("#edit-school-features [name='features.greatForWestern']").prop("checked", data.data.features.greatForWestern);
        $("#edit-school-features [name='features.acceptsBeginners']").prop("checked", data.data.features.acceptsBeginners);
        $("#edit-school-features [name='features.wifiInCommonAreas']").prop("checked", data.data.features.wifiInCommonAreas);
        $("#edit-school-features [name='features.studentLounge']").prop("checked", data.data.features.studentLounge);
        $("#edit-school-features [name='features.studentLounge24hour']").prop("checked", data.data.features.studentLounge24hour);
        $("#edit-school-features [name='features.englishStaff']").prop("checked", data.data.features.englishStaff);
        $("#edit-school-features [name='features.fullTimeJobSupport']").prop("checked", data.data.features.fullTimeJobSupport);
        $("#edit-school-features [name='features.partTimeJobSupport']").prop("checked", data.data.features.partTimeJobSupport);
        $("#edit-school-features [name='features.interactWithJapanese']").prop("checked", data.data.features.interactWithJapanese);
        $("#edit-school-features [name='features.slowPaced']").prop("checked", data.data.features.slowPaced);
        $("#edit-school-features [name='features.smallClassSizes']").prop("checked", data.data.features.smallClassSizes);
        $("#edit-school-features [name='features.studentDorms']").prop("checked", data.data.features.studentDorms);
        $("#edit-school-features [name='features.uniqueFeature']").prop("checked", data.data.features.uniqueFeature);

        tinymce.EditorManager.get('uniqueFeatureText').setContent("");
        tinymce.EditorManager.get('uniqueFeatureText').setContent(getContent(data.data.content, "uniqueFeatureText", "en"));
        $("#edit-school-features [name='uniqueFeature']").val(getContent(data.data.content, "USP", "en"));

        //location section
        if (data.data.locationID != "undefined") {
            $("[data-locationselect]").val(data.data.locationID);
        } else {
            $("[data-locationselect]").val(0);
        }
        refreshLocation();
    
        doStationList(data.data.schoolStations);


        //edit course general

        $("#editCourseGeneral").attr("action", "/api/addCourseGeneral/" + data.data.id + "/getSchoolList")

        $("[name=coursesHeading]").val(coursesHeading);

        if (data.data.coursesImage != null && data.data.coursesImage != "") {
            $("#editCourseGeneral .image").html("");
            $("#editCourseGeneral .image").css("background-image", "url('" + data.data.coursesImage + "?" + Math.floor(Math.random() * 100) + "')");
            $("#editCourseGeneral .image").css("height", "150px");
        } else {
            $("#editCourseGeneral .image").html("no image");
            $("#editCourseGeneral .image").css("background-image", "none");
            $("#editCourseGeneral .image").css("height", "auto");
        }

       
        $("#addCourseForm").attr("action", "/api/addCourse/" + data.data.id + "/getSchoolList");

    }



    function doStationList(stations) {

        $("#addSchoolStation [name=name]").val("");
        $("#addSchoolStation [name=line]").val("");
        $("#addSchoolStation [name=distance]").val("");

        if (!stations.length) {
            $("#stationListHolder").html("<div class='text-center'><i class='icon icon-exclamation-triangle orange' ></i> no stations added</div>");
        } else {

            $("#stationListHolder").html("");

        
            for (var i = 0; i < stations.length; i++) {
                var form = $("#stationClone form").clone(false);
                //form.off();
                var container = $("#stationListHolder");

                form.attr("id", "station" + i);
                form.attr("action", "/api/editSchoolStation/" + stations[i].id + "/" + stations[i].schoolID + "/getSchool");

                form.find("[name=name]").val(stations[i].name);
                form.find("[name=line]").val(stations[i].line);
                form.find("[name=distance]").val(stations[i].distance);
                form.find("[data-removeSchoolStation]").attr("data-id", stations[i].id);

                if (stations[i].image != null && stations[i].image != "") {
                    form.find(".image").html("");
                    form.find(".image").css("background-image", "url('" + stations[i].image + "?" + Math.floor(Math.random() * 100) + "')");
                    form.find(".image").css("height", "150px");
                } else {
                    form.find(".image").html("no image");
                    form.find(".image").css("background-image", "none");
                    form.find(".image").css("height", "auto");
                }


                if ((i + 1) < stations.length) {
                    form.append("<div class='col-xs-12'><hr /></div>");
                }
                container.append(form);
            }
            //bindAjaxSubmit();

            $("[data-removeSchoolStation]").on("click", function (e) {
                var stationID = $(this).attr("data-id");

                e.preventDefault();
                $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this station?");
                $('#confirmDeleteModal').modal()
                    .one('click', '[data-confirmedDelete]', function () {
                        $('#confirmDeleteModal').modal("hide");
                        ajaxSubmit("post", "/api/deleteSchoolStation/" + stationID, null, "getSchool");
                    });
            });
        }

   
    }


    function getCourses() {
        //$("[data-courseSelect]").html('<option value="0">select course</option><option disabled>-----</option>');
        var schoolID;
        if ($("[data-schoollistcourse]").val() != 0 && $("[data-schoollistcourse]").val() != null) {
            schoolID = $("[data-schoollistcourse]").val();
            $("#addCourseForm").attr("action", "/api/addCourse/" + schoolID + "/getCourses");
        }
        ajaxSubmit("post", "/api/getCourses/" + schoolID, null, "fillCourseList");
    }

    function fillCourseList(data) {
        $("[data-courseSelect]").html('<option value="0">select course</option><option disabled>-----</option>');
        for (var i = 0; i < data.data.length; i++) {
            var name;
            for (var j = 0; j < data.data[i].courseContent.length; j++) {
                if (data.data[i].courseContent[j].name == "name") {
                    name = data.data[i].courseContent[j].contentCollection[0].body;
                    $("[data-courseSelect]").append("<option value='" + data.data[i].id + "'>" + name + "</option>");
                }
           
            }

            //getCourse();
        }

    }

    function getLocations() {
   
        $("#editSchoolLocation [data-locationselect]").html('<option value="0">select location</option><option disabled>-----</option>');
        ajaxSubmit("post", "/api/getLocations/en", null, "fillLocationList");
    }

    function fillLocationList(data) {
        for (var i = 0; i < data.data.length; i++) {
            $("#editSchoolLocation [data-locationselect]").append("<option value='" + data.data[i].id + "'>" + data.data[i].name + "</option>");
        }
    
        getSchoolList();
    }


    function loadLocation(data) {
    
    
        var content = data.data.content;
        var description;

        for (var i = 0; i < content.length; i++) {
            if (content[i].name == "description") {
                description = content[i].body;
            }
        }

        $("#editSchoolLocation").attr("action", "/api/editSchoolLocation/" + $("[data-schoollist]").val() + "/" + data.data.locationID + "/refreshLocation");

        if (data.data.image != null && data.data.image != "") {
            $("#editSchoolLocation .image").html("");
            $("#editSchoolLocation .image").css("background-image", "url('" + data.data.image + "?" + Math.floor(Math.random() * 100) + "')");
            $("#editSchoolLocation .image").css("height", "150px");
        } else {
            $("#editSchoolLocation .image").html("no image");
            $("#editSchoolLocation .image").css("background-image", "none");
            $("#editSchoolLocation .image").css("height", "auto");
        }
        
        $("#editSchoolLocation [name=name]").val(data.data.name);

        if (description == null) {

            tinymce.EditorManager.get('description').setContent("");

        } else {
            tinymce.EditorManager.get('description').setContent(description);
        }


    }

    function manageUserSetup() {
        $("[data-manageUser]").on("click", function () {
           
            $(this).closest(".userRow").find(".userManagementBox").toggle(300);
        })


        $("[data-btnassignrole]").on("click", function () {
            var userID = $(this).attr("data-userid");
            var roleID = $(this).closest(".userManagementBox").find(".roleSelector").val();
            var formData = new FormData();
            ajaxSubmit("post", "/api/addRole/" + userID+"/"+roleID, formData, "loadUserList");

        });

        $("[data-removerole]").on("click", function () {
            var userID = $(this).attr("data-userid");
            var roleID = $(this).closest(".userManagementBox").find(".roleSelector").val();
            var formData = new FormData();
            ajaxSubmit("post", "/api/removeRole/" + userID + "/" + roleID, formData, "loadUserList");

        });


        $("[data-deleteUser]").on("click", function (e) {
            var userID = $(this).attr("data-userid");
            var formData = new FormData();
        
            e.preventDefault();
            $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this user?");
            $('#confirmDeleteModal').modal()
                .one('click', '[data-confirmedDelete]', function () {
                    $('#confirmDeleteModal').modal("hide");
                    ajaxSubmit("post", "/api/deleteUser/" + userID, formData, "loadUserList");
                });
        });



        $("[data-btnassignlanguage]").on("click", function () {
            var userID = $(this).attr("data-userid");
            var code = $(this).closest(".userManagementBox").find(".languageSelector").val();
            var formData = new FormData();
            ajaxSubmit("post", "/api/assignLanguage/" + code + "/" + userID, formData, "loadUserList");
        });

        $("[data-removelanguage]").on("click", function () {
            var userID = $(this).attr("data-userid");
            var languageCode = $(this).closest(".userManagementBox").find(".languageSelector").val();
            var formData = new FormData();
            ajaxSubmit("post", "/api/unassignLanguage/" + languageCode + "/" + userID, formData, "loadUserList");
        });


    }


    function loadRoleList() {
        ajaxRequest("post", "/api/getRoles/", null, fillRoleList);
    }

    function loadRoleListAfterChanges() {
        ajaxRequest("post", "/api/getRoles/", null, fillRoleList);
        loadUserList();
    }


    function fillRoleList(data) {
        $("[data-roleList]").html("");
        for (var i = 0; i < data.data.length; i++) {
            $("[data-roleList]").append("<li>" + data.data[i].Name + "<button data-deleteRole data-name='" + data.data[i].Name + "' class='btn btn-danger btn-xs' data-Id='" + data.data[i].Id + "'>delete</button></li> ");
        }

        $("[data-deleteRole]").on("click", function (e) {
            var roleID = $(this).attr("data-Id");
            var name = $(this).attr("data-name");
            var formData = new FormData();

            e.preventDefault();
            $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete the '<strong>" + name + "</strong>' role?");
            $('#confirmDeleteModal').modal()
                .one('click', '[data-confirmedDelete]', function () {
                    $('#confirmDeleteModal').modal("hide");
                    ajaxSubmit("post", "/api/deleteRole/" + roleID, formData, "loadRoleListAfterChanges");
                });
        });

    }





    function refreshContentList() {
        
        getContentList();
    }


    function doAdminLanguageIcons(){
        //code for admin language icons

        $("[data-language-icon]").on("click", function () {
           
            var contentID = $("#contentSelect").val();
            var code = $(this).attr("data-id");
            var contentBodyID = $(this).attr("data-contentBodyID");
            var title = $(this).attr("data-name");

            $('#editContentModal').modal('show');
    
            $("#editContentBox textarea").val("");
            $("#editContentBox input[name=contentID]").val(contentID);
            $("#editContentBox input[name=code]").val(code);
            $("#editContentBox input[name=contentBodyID]").val(contentBodyID);
            $('#editContentModal .modal-title').html("Editing " + $("#contentSelect option:selected").html() + " " + title + " content");
            //bindEditors();

            tinyMCE.activeEditor.setContent('');
            ajaxRequest("post", "/api/getContentBodyByCode/" + contentID + "/" + code + "/showContent", null, null);
      
       
        
        })
    }

    function setupIcons(data) {

        $(".admin-language-icon").each(function () {

            for (var s = 0; s < data.data[0].contentCollection.length; s++) {
                if ($(this).attr("data-id") == data.data[0].contentCollection[s].code && data.data[0].contentCollection[s].body != "content not found") {
                    
                    $(this).removeClass("faded");
                }
            }

        });
    }


    function showContent(data) {
    
        if (data.data.length) {

            tinymce.EditorManager.get('pageContentEditor').setContent(data.data[0].body);
            //tinyMCE.getEditor("pageContentEditor").setContent(data.data[0].body);
            $("#editContentBox input[name=contentBodyID]").val(data.data[0].contentBodyID);
        }
    
    
    }


    function getLanguageList() {

        $("#newLanguageForm input").each(function () {
            $(this).val("");
        });
        ajaxRequest("post", "/api/getLanguageList/", null, fillLanguageList);
    }


    function getPageList() {

        $("#newPageForm input, #newPageForm textarea").each(function(){
            $(this).val("");
        });
        ajaxRequest("post", "/api/getPageList/", null, fillPageList);
    }


    function getContentList() {
        blankContent();
        $("#newPageForm input").each(function () {
            $(this).val("");
        });
   
        ajaxRequest("post", "/api/getContentList/" + $("#pageSelect").val(), null, fillContentList);
    }

    function fillPageList(data) {

        $("#pageSelect").html("");
        for (var i = 0; i < data.data.length; i++) {
            $("#pageSelect").append("<option value='" + data.data[i].pageID + "'>" + data.data[i].name + "</option> ");
        }

        if ($("#pageSelect").val() != null) {
            $("#addPageGallery").attr("action", "api/addPageGallery/" + $("#pageSelect").val() + "/getPageContent");
            ajaxRequest("post", "/api/getPage/" + $("#pageSelect").val(), null, fillPage);
        }
    

        //update hidden pageID for adding new content on page admin content section
        if ($("#addPageContent").length) {
            $("#addContentPageID").val($("#pageSelect").val());
        }


    }


    function fillLanguageList(data) {

        $("#languageSelect").html("");
        for (var i = 0; i < data.data.length; i++) {
            $("#languageSelect").append("<option value='" + data.data[i].code + "'>" + data.data[i].name + "</option> ");
        }

        ajaxRequest("post", "/api/getLanguage/" + $("#languageSelect").val(), null, fillLanguage);

    }


    function fillContentList(data) {

        $("#contentSelect").html("");
        tinymce.EditorManager.get('pageContentEditor').setContent("");
        for (var i = 0; i < data.data.length; i++) {
            $("#contentSelect").append("<option value='" + data.data[i].contentID + "'>" + data.data[i].name + "</option> ");
        }

        if ($("#contentSelect").val() != null) {
            ajaxRequest("post", "/api/getContentByID/" + $("#contentSelect").val(), null, fillContent);
        }
        

    }



    //update content data 

    function refreshContent() {
        
        ajaxRequest("post", "/api/getContentByID/" + $("#contentSelect").val(), null, fillContent);
        $("#editContentModal").modal('hide');
    
    }


    function fillPage(data) {
        $("#addPageGallery").attr("action", "api/addPageGallery/" + $("#pageSelect").val() + "/getPageContent");
        $("#addPageFaq").attr("action", "api/addPageFaq/" + $("#pageSelect").val() + "/getPageContent"); 
        
        $("[data-contenteditname]").val("");
        $("[data-language-icon]").html("");
        ajaxRequest("post", "/api/getLanguageList/", null, setupLanguages);
        $("#addContentName").val("");


        $("[data-pageID]").html(data.data.pageID);
        $("[data-pageName]").val(data.data.name);
        $("[data-showPageID]").html(data.data.pageID);
        $("[data-pageDescription]").val(data.data.description);
    
   
        if ($("#pageSelect").val() != "") {
            getContentList();
            getPageContent();
        }
    
        //update hidden pageID for adding new content on page admin content section
        if ($("#addPageContent").length) {
            $("#addContentPageID").val($("#pageSelect").val());
        }

    }

    function getPageContent() {
        ajaxRequest("post", "/api/getPageContent/" + $("#pageSelect").val(), null, fillPageContent);
    }

    function fillPageContent(data) {
        var faqs = data.data.faqs;

        $("#addPageGallery [name=name]").val("");
        $("#addPageFaq [name=name]").val("");

        $("#FAQSelect *").remove();
        var container = $("#FAQSelect");
        var html = '';

        html = $('<div class="row">\
						<div class="form-group col-xs-12">\
							<div class="input-group">\
								<div class="input-group-addon black"><i class="icon icon-list"></i> select faq</div>\
								<select data-pageFaqList="" class="form-control input-sm" name="id">\
								</select>\
								<span class="input-group-btn">\
									<button class="btn btn-danger btn-sm" data-deleteFaq type="button">delete</button>\
								</span>\
							</div>\
						</div>\
					</div>');


        for (var i = 0; i < faqs.length; i++) {
            html.find("[data-pageFaqList]").append("<option value='" + faqs[i].id + "'>" + faqs[i].name + "</option>");
        }

        container.append(html);

        setupAddFAQQForm();
        setupPageFAQs();
        

        $("[data-pagefaqlist]").on("change", function () {
            
            ajaxRequest("post", "/api/getQuestions/" + $("[data-pagefaqlist]").val(), null, doPageFaqs);
            setupAddFAQQForm();
        });


        $("[data-deleteFaq]").off();
        $("[data-deleteFaq]").on("click", function (e) {
            var id = $("[data-pagefaqlist]").val();
            var pageID = $("#pageSelect").val();
            e.preventDefault();
            $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this faq?");
            $('#confirmDeleteModal').modal()
                .one('click', '[data-confirmedDelete]', function () {
                    $('#confirmDeleteModal').modal("hide");
                    ajaxSubmit("post", "/api/deletePageFaq/"+pageID+"/" + id, null, "afterGalleryRefresh");
                });
        });
        


        /// setup gallery list
        $("#editPageGalleries *").remove();
        container = $("#editPageGalleries");
        var galleries = data.data.galleries;
        
        for (var i = 0; i < galleries.length; i++) {

            var name = getContent(galleries[i].content, "name", "en");
            var count = galleries[i].galleryImages.length;

            html = '<div class="input-group galleryList-row">\
					<div class="input-group-addon black"><i class="icon icon-camera"></i>'+ name + '</div>\
					<div class="input-group-addon" style="width:100%">'+ count + ' images</div>\
					<span class="input-group-btn">\
                        <button class="btn btn-primary btn-sm" type="button" data-id="' + galleries[i].id + '" data-editGallery>edit</button>\
                        <button class="btn btn-danger btn-sm" type="button" data-id="' + galleries[i].id + '" data-deleteGallery>delete</button>\
					</span>\
				</div>';
            container.append(html);


        }


        $("[data-editGallery]").off();
        $("[data-editGallery]").on("click", function (e) {
            var id = $(this).attr("data-id");
            e.preventDefault();
            getGalleryFormData(id);
        });


        $("[data-deleteGallery]").on("click", function (e) {
            var id = $(this).attr("data-id");
            e.preventDefault();
            $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this gallery?");
            $('#confirmDeleteModal').modal()
                .one('click', '[data-confirmedDelete]', function () { 
                    $('#confirmDeleteModal').modal("hide");
                    ajaxSubmit("post", "api/deletePageGallery/" + $("#pageSelect").val() + "/" +id, null, "afterGalleryRefresh");
                });
        });


    }

    function doPageFaqs(questions) {

        //setup faqs
        questions = questions.data;
        html = '';

        $("#FAQContainer *").remove();
        var container = $("#FAQContainer");

       

            for (var j = 0; j < questions.length; j++) {

                var question;
                var answer;

                question = getContent(questions[j].content, "question", "en");
                answer = getContent(questions[j].content, "answer", "en");

                var html = '<div class="FAQQuestion">\
								<div class="panel panel-default panel-xs">\
									<div class="panel-heading clearfix">\
										<h4 class="panel-title"><i class="icon icon-question-circled"></i> question '+ (j + 1) + '</h4>\
										<div class="icon">\
											<i class="icon icon-plus-circled"></i>\
										</div>\
									</div>\
									<div class="panel-body">\
										<div class="row">\
											<div class="form-group col-xs-12">\
												<div class="input-group">\
													<div class="input-group-addon black"><i class="icon icon-question-circled"></i>question</div>\
													<input class="form-control input-sm" name="question" value="'+ question + '" required>\
												</div>\
											</div>\
										</div>\
										<div class="row">\
											<div class="form-group col-xs-12">\
												<div class="input-group-addon black single"><i class="icon icon-comment"></i>answer</div>\
												<textarea class="form-control" data-wysiwyg-xs name="answer" id="answer' + questions[j].id + '">' + answer + '</textarea>\
											</div>\
										</div>\
										<div class="row">\
											<div class="form-group col-xs-12">\
												<div class="input-group input-group-sm">\
													<div style="width:100%"></div>\
													<div class="input-group-addon black "><i class="icon icon-cog"></i></div>\
													<span class="input-group-btn">\
														<button type="button" data-updateFAQQuestion data-id="' + questions[j].id + '" class="btn btn-info btn-xs">edit</button>\
														<button type="button" data-deleteFAQQuestion data-id="' + questions[j].id + '" class="btn btn-danger btn-xs">delete</button>\
													</span>\
												</div>\
											</div>\
										</div>\
									</div>\
								</div>';

                container.append(html);
            }

   

        doAdminPanels();
        bindEditors();


        $("[data-updateFAQQuestion]").off();
        $("[data-updateFAQQuestion]").on("click", function () {
            var id = $(this).attr("data-id");
            var form = new FormData();
            form.append("question", $(this).closest(".FAQQuestion").find("[name=question]").val());
            form.append("answer", tinymce.EditorManager.get("answer" + id).getContent());
            ajaxSubmit("post", "/api/editQuestion/" + id, form, null);
        });


        $("[data-deletefaqquestion]").off();
        $("[data-deletefaqquestion]").on("click", function (e) {
            var id = $(this).attr("data-id");
            e.preventDefault();
            $('#confirmDeleteModal').find(".modal-body").html("Are you sure you want to delete this question?");
            $('#confirmDeleteModal').modal()
                .one('click', '[data-confirmedDelete]', function () {
                    $('#confirmDeleteModal').modal("hide");
                    ajaxSubmit("post", "/api/deleteQuestion/" + id, null, "afterGalleryRefresh");
                });
        });

    }


    function setupPageFAQs() {

        $("#addFaqQuestion [name=question]").val("");
        if (tinymce.EditorManager.get("addFaqAnswer") != null) {
            tinymce.EditorManager.get("addFaqAnswer").setContent("");
        }

        if ($("[data-pagefaqlist]").val() != null) {
            ajaxRequest("post", "/api/getQuestions/" + $("[data-pagefaqlist]").val(), null, doPageFaqs);
        } else {
            $("#FAQContainer *").remove();
        }
    }

    function setupAddFAQQForm() {
        $("#addFaqQuestion").attr("action", "api/addPageQuestion/" + $("#pageSelect").val() + "/" + $("[data-pagefaqlist]").val() + "/setupPageFAQs");
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


    function fillLanguage(data) {
        $("[data-languageEditName]").val(data.data[0].name);
        $("[data-languageEditCode]").val(data.data[0].code);
        $("#show-flag").html("<img class='img-responsive' style='display:inline' src='" + data.data[0].flag + "' />");

        $("[data-deleteLanguage]").off();
        $("[data-deleteLanguage]").on("click", function (e) {
            var id = $("#languageSelect").val();
            e.preventDefault();
            $('#confirmDeleteModal').find(".modal-body").html("This is very dangerous and will delete ALL contenet for this language, are you sure?");
            $('#confirmDeleteModal').modal()
                .one('click', '[data-confirmedDelete]', function () { 
                    $('#confirmDeleteModal').modal("hide");
                    ajaxSubmit("post", "/api/deleteLanguage/" + id, null, "getLanguageList");
                });
        });
       
    }


    function setupLanguages(data) {

        //create icons and language slection
        $("[data-language-icons]").html("");


        //getLanguages
        for (var i = 0; i < data.data.length; i++) {
            $("[data-language-icons]").append('<div class="admin-language-icon faded" data-language-icon data-id="'+data.data[i].code+'" data-name=""><img class="img-responsive" src="' + data.data[i].flag + '" /></div> ');
        }

    
        doAdminLanguageIcons();


    }


    function fillContent(data) {
        tinymce.EditorManager.get('pageContentEditor').setContent("");
        $(".admin-language-icon").addClass("faded");
        if (data.data[0] != null) {
            $("[data-contentEditName]").val(data.data[0].name);
            $("#editContentPageID").val($("#pageSelect").val());
            setupIcons(data);
        
            
            //setup content delete
            $("#deleteContentPageID").val(data.data[0].pageID);
            $("#deleteContentContentID").val(data.data[0].contentID);

        }
    
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
          
              if (data.statusCode <= 1) {
             
                  if (callback != null) {
                      callback(data);
                  } else if (data.callback != null) {
                      window[data.callback](data);
                  }
                
                
              }
          
              if (data.message != null) {

                  var alertType;

                  if (data.statusCode <= 1) {
                      alertType = "success";
                      alertHeading = "Success"
                  } else {
                      alertType = "warning";
                      alertHeading = "Warning"
                  }

                  $("#info-bar").html('<div class="alert alert-' + alertType + ' alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + alertHeading + '</strong> ' + data.message + '</div>');
                  $('html, body').animate({
                      scrollTop: $("#info-bar").offset().top + -100
                  }, 400);
             
              }
          
          })
            .fail(function (jqXHR, textStatus) {
                //add dailog stuff
                $("#info-bar").html('<div class="alert alert-warning alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>Warning!</strong> Ajax failed, code is ' + jqXHR.status + '.  Call Batman.</div>');
                $('html, body').animate({
                    scrollTop: $("#info-bar").offset().top + -100
                }, 400);

            });

  
    }


    function ajaxSubmit(method, action, data, callback) {
   
        $.ajax({
            method: method,
            url: action,
            data: data,
            contentType: false,
            processData: false
        })
          .done(function (data, status, jqXHR) {
              if (callback == null) {
                  callback = data.callback;
              }

              if (data.statusCode == "1" || data.statusCode == "3" || callback != null) {
                  
                  window[callback](data);
              }

              if (data.message != null) {

                  var alertType;

                  if (data.statusCode <= 1) {
                      alertType = "success";
                      alertHeading = "Success"
                  } else {
                      alertType = "warning";
                      alertHeading = "Warning"
                  }

                  $("#info-bar").html('<div class="alert alert-' + alertType + ' alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + alertHeading + '</strong> ' + data.message + '</div>');
                  $('html, body').animate({
                      scrollTop: $("#info-bar").offset().top + -100
                  }, 400);
              }
          })
            .fail(function (jqXHR, textStatus) {
                $("#info-bar").html('<div class="alert alert-warning alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>Warning!</strong> Ajax failed, code is ' + jqXHR.status + '.  Call Batman.</div>');
                $('html, body').animate({
                    scrollTop: $("#info-bar").offset().top + -100
                }, 400);
            });

    }


    //makes tinymce plugins work properly after they have been hidden via css.
    $(document).on('focusin', function (e) {
        if ($(e.target).closest(".mce-window").length) {
            e.stopImmediatePropagation();
        }
    });

    function bindEditors() {
 
 
        tinymce.baseURL = "/Scripts/tinymce";
        tinymce.init({
            selector: "[data-wysiwyg]",
            height : "200",
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
               { title: "Lilita One", inline: "span", styles: { 'font-family': 'Lilita One' } },
               { title: "Lilita One Medium", inline: "span", styles: { 'font-family': 'Lilita One', 'font-size': '42px' } },
               { title: "Lilita One large", inline: "span", styles: { 'font-family': 'Lilita One', 'font-size': '118px' } }
            ]
        }
            ],
            theme: "modern",
            valid_elements: "*[*]",
            plugins: [
                "advlist autolink lists link image charmap print preview hr anchor pagebreak",
                "searchreplace fullscreen",
                "insertdatetime media nonbreaking save table contextmenu directionality",
                "template paste textcolor colorpicker textpattern imagetools code"
            ],
            toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
            toolbar2: " preview media | forecolor backcolor emoticons | code",
            image_advtab: true
        });


        tinymce.baseURL = "/Scripts/tinymce";
        tinymce.init({
            selector: "[data-wysiwyg-xs]",
            height: "100",
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
               { title: "Lilita One", inline: "span", styles: { 'font-family': 'Lilita One' } },
               { title: "Lilita One Medium", inline: "span", styles: { 'font-family': 'Lilita One', 'font-size': '42px' } },
               { title: "Lilita One large", inline: "span", styles: { 'font-family': 'Lilita One', 'font-size': '118px' } }
            ]
        }
            ],
            theme: "modern",
            valid_elements: "*[*]",
            plugins: [
                "advlist autolink lists link image charmap print preview hr anchor pagebreak",
                "searchreplace fullscreen",
                "insertdatetime media nonbreaking save table contextmenu directionality",
                "template paste textcolor colorpicker textpattern imagetools code"
            ],
            toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
            toolbar2: " preview media | forecolor backcolor emoticons | code",
            image_advtab: true
        });

    }





