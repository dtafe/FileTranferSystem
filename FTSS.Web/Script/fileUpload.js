function sendFileToServer(formData, status, name) {
    var uploadURL = "Upload.aspx"; //Upload URL
    var extraData = {}; //Extra Data.
    var jqXHR = $.ajax({
        xhr: function () {
            var xhrobj = $.ajaxSettings.xhr();
            if (xhrobj.upload) {
                xhrobj.upload.addEventListener('progress', function (event) {
                    var percent = 0;
                    var position = event.loaded || event.position;
                    var total = event.total;
                    if (event.lengthComputable) {
                        percent = Math.ceil(position / total * 100);
                    }
                    //Set progress
                    status.setProgress(percent);
                }, false);
            }
            return xhrobj;
        },
        url: uploadURL,
        type: "POST",
        contentType: false,
        processData: false,
        cache: false,
        dataType: "json",
        data: formData,
        success: function (data) {
            status.setProgress(100);
        }
    });
    status.setAbort(jqXHR, name);
}


var rowCount = 0;
function createStatusbar(obj) {
    rowCount++;
    var row = "odd";
    if (rowCount % 2 == 0) row = "even";

    this.statusbar = $("<div class='statusbar " + row + "'></div>");
    this.filename = $("<div class='filename'></div>").appendTo(this.statusbar);
    this.size = $("<div class='filesize'></div>").appendTo(this.statusbar);
    this.progressBar = $("<div class='progressBar'><div></div></div>").appendTo(this.statusbar);
    this.abort = $("<div class='abort'><img src='../Images/Delete-icon.png' /></div>").appendTo(this.statusbar);


    obj.after(this.statusbar);

    this.setFileNameSize = function (name, size) {
        var sizeStr = "";
        var sizeKB = size / 1024;
        if (parseInt(sizeKB) > 1048576) {
            var sizeGB = sizeKB / 1048576;
            sizeStr = sizeGB.toFixed(2) + " GB";
        }
        else if (parseInt(sizeKB) > 1024 && parseInt(sizeKB) < 1048576) {
            var sizeMB = sizeKB / 1024;
            sizeStr = sizeMB.toFixed(2) + " MB";
        }
        else {
            sizeStr = sizeKB.toFixed(2) + " KB";
        }

        this.filename.html(name);
        this.size.html(sizeStr);
    }
    this.setProgress = function (progress) {
        var progressBarWidth = progress * this.progressBar.width() / 100;
        this.progressBar.find('div').animate({ width: progressBarWidth }, 100).html(progress + "% ");
        if (parseInt(progress) >= 100) {
            this.abort.show();
        }
    }
    this.setAbort = function (jqxhr, name) {
        var sb = this.statusbar;
        this.abort.click(function () {
            jqxhr.abort();
            $.ajax({
                type: "POST",
                url: "Upload.aspx/DeleteFile",
                data: "{'filename':'" + name + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            });
            sb.hide();
        });
    }
}

    
    function handleFileUpload(files, obj) {
        for (var i = 0; i < files.length; i++) {
            var fd = new FormData();
            fd.append('file', files[i]);
            var status = new createStatusbar(obj); //Using this we can set progress.
            status.setFileNameSize(files[i].name, files[i].size);
            sendFileToServer(fd, status, files[i].name);

        }
    }

    $(document).ready(function () {
        var obj = $("#dragandrophandler");
        obj.on('dragenter', function (e) {
            e.stopPropagation();
            e.preventDefault();
            $(this).css('border', '2px dashed #ca5100');
        });
        obj.on('dragover', function (e) {
            e.stopPropagation();
            e.preventDefault();
        });
        obj.on('drop', function (e) {

            $(this).css('border', '2px dashed #ca5100');
            e.preventDefault();
            var files = e.originalEvent.dataTransfer.files;
            //send dropped files to Server
            handleFileUpload(files, obj);
        });

        $(document).on('dragenter', function (e) {
            e.stopPropagation();
            e.preventDefault();
        });
        $(document).on('dragover', function (e) {
            e.stopPropagation();
            e.preventDefault();
            obj.css('border', '2px dashed #ca5100');
        });
        $(document).on('drop', function (e) {
            e.stopPropagation();
            e.preventDefault();
        });

        //Click choose file upload  
        $(dragandrophandler).on('click', function (e) {
            e.preventDefault();
            $('#fileInput').trigger('click');
            return false;
        });
        $('#fileInput').on('change', function (e) {
            e.preventDefault();
            if (this.files.length === 0) return;
            var files = $('#fileInput').prop('files');
            handleFileUpload(files, obj);
        });
    });
    
