$(async function() {
    let loginInfo = await app.checkLogin();
    app.checkPageAllow(loginInfo?.data?.account?.role, ['MONITOR','ADMIN','ROOT']);
    $('#accountName').text(`Xin chào, ${loginInfo?.data?.account?.username}`);
    var table = $('#table-picked-student');
    var ntable = $('#table-new-pick-student');
    var row = $('#tb-row');
    var emptyRow = $('<tr><td colspan="8" class="text-center">Chưa có dữ liệu</td></tr>');
    var isFetch = false;
    var msg = new SpeechSynthesisUtterance();
    var voices = window.speechSynthesis.getVoices();
    var lang = "vi-VN";
    console.log(voices);
    msg.volume = 10; // âm lượng (1 - 10)
    msg.rate = 0.7; // tốc độ nói (0.1 - 10) 
    msg.pitch = 1; // độ cao (0 - 2) 
    msg.lang = lang; // set ngôn ngữ
    msg.voice = voices.filter(voice => voice.lang == lang)[0];
    // ở đây mình chọn tiếng anh giọng nam
     
    async function getData() {
        var dash = await app.api.get('/rest/dashboard');
        if (dash && dash.data) {
            $('.pickdate').text(dash.data.date);
            $('#totalCount').text(`Đã đón: ${dash.data.pickedStudent} - Tổng số: ${dash.data.totalStudent}`);
            if (dash.data.newPickup && dash.data.newPickup.length) {
                isFetch = true;
                setTimeout(async () => {
                    for (let i = 0; i < dash.data.newPickup.length; i++) {
                        const e = dash.data.newPickup[i];
                        var r = row.clone();
                        r.find('.index').text(i + 1);
                        r.find('.sname').text(e.student.name);
                        r.find('.sclass').text(e.student.class);
                        r.find('.sgrade').text(e.student.grade);
                        r.find('.gname').text(e.guardian.name);
                        r.find('.gphone').text(e.guardian.phone);
                        r.find('.pdate').text(app.displayFormat(new Date(e.date)));
                        r.find('.state').text(e.state == 'REQUEST' ? 'Phụ huynh đang chờ' : 'Đã đón');
                        r.addClass('blink_me');
                        ntable.html(r.prop('outerHTML'));
                        msg.text = `Đón ${e.student.name} lớp ${e.student.class}`; // nội dung
                        speechSynthesis.speak(msg);
                        await app.delay(10000);
                    }
                    await getData();
                    isFetch = true;
                }, 100);
            } else {
                isFetch = false;
                ntable.html(emptyRow.prop('outerHTML'));
            }

            if (dash.data.showPickup && dash.data.showPickup.length) {
                let html = '';
                dash.data.showPickup.forEach((e, i) => {
                    var r = row.clone();
                    r.addClass(e.state == 'REQUEST' ? 'table-success': 'table-light');
                    r.find('.index').text(i + 1);
                    r.find('.sname').text(e.student.name);
                    r.find('.sclass').text(e.student.class);
                    r.find('.sgrade').text(e.student.grade);
                    r.find('.gname').text(e.guardian.name);
                    r.find('.gphone').text(e.guardian.phone);
                    r.find('.pdate').text(app.displayFormat(new Date(e.updatedDate || e.date)));
                    r.find('.state').text(e.state == 'REQUEST' ? 'Phụ huynh đang chờ' : 'Đã đón');
                    html += r.prop('outerHTML');
                });
                table.html(html);
            } else {
                table.html(emptyRow.prop('outerHTML'));
            }

        } else {
            table.html(emptyRow.prop('outerHTML'));
            ntable.html(emptyRow.prop('outerHTML'));
        }
        if (!isFetch) {
            await app.delay(5000);
            await getData();
        }
    }

    getData();
})