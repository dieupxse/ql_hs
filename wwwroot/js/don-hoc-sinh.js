$(async function () {
    let loginInfo = await app.checkLogin();
    app.checkPageAllow(loginInfo?.data?.account?.role, 'GUARDIAN');
    $('#accountName').text(`Xin chào, ${loginInfo?.data?.guardian?.name}`);

    var studentList = $('#student-list');
    var studentListPicked = $('#student-list-picked');
    var studentInfo = $('#student-info');
    studentList.html('');
    if (loginInfo?.data?.studentNeedPickup?.length) {
        loginInfo?.data?.studentNeedPickup?.forEach((e) => {
            var stu = studentInfo.clone();
            stu.find('.student-name').text(e.name);
            stu.find('.student-class').text(e.class);
            stu.find('.student-grade').text(e.grade);
            stu.find('.btn-pickup').attr('data-id', e.id);
            stu.find('.btn-pickup').attr('data-gid', e.guardianId);
            studentList.append(stu);
        });
    } else {
        studentList.html('<div class="col-md-12"><div class="alert alert-info">Không có học sinh nào cần đón hôm nay.</div></div>');
    }
    if (loginInfo?.data?.picked?.length) {
        loginInfo?.data?.picked?.forEach((e) => {
            var stu = studentInfo.clone();
            stu.find('.student-name').text(e.student.name);
            stu.find('.student-class').text(e.student.class);
            stu.find('.student-grade').text(e.student.grade);
            stu.find('.btn-pickup').attr('disabled', 'disabled');
            stu.find('.btn-pickup').text(`Đã đón lúc ${app.displayFormat(new Date(e.date))}`);
            studentListPicked.append(stu);
        });
    } else {
        studentListPicked.html('<div class="col-md-12"><div class="alert alert-info">Chưa đón học sinh nào hôm nay.</div></div>');
    }

    $('.btn-pickup').click(async function (e) {
        e.preventDefault();
        var studentId = $(this).attr('data-id');
        var guardianId = $(this).attr('data-gid');
        var res = await app.api.post('/rest/pickup', {
            studentId: studentId,
            guardianId: guardianId
        });
        if (res && res?.errorCode == 200) {
            alert('Đón học sinh thành công!');
            window.location.reload();
        } else {
            alert('Đã xảy ra lỗi');
        }
    })
})