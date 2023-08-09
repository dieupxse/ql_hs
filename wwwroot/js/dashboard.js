$(async function() {
    let loginInfo = await app.checkLogin();
    app.checkPageAllow(loginInfo?.data?.account?.role, ['ROOT', 'ADMIN']);
    $('#accountName').text(`Xin chào, ${loginInfo?.data?.account?.username}`);
    var table = $('#table-picked-student');
    var ntable = $('#table-new-pick-student');
    var row = $('#tb-row');
    var emptyRow = '<tr><td colspan="7" class="text-center">Chưa có dữ liệu</td></tr>';
    table.html('');
    ntable.html('');
    var isFetch = false;
    async function getData() {
        table.html('');
        var dash = await app.api.get('/rest/dashboard');
        if (dash && dash.data) {
            $('.pickdate').text(dash.data.date);
            $('#totalCount').text(`Đã đón: ${dash.data.pickedStudent} - Tổng số: ${dash.data.totalStudent}`);
            if (dash.data.newPickup && dash.data.newPickup.length) {
                isFetch = true;
                setTimeout(async () => {
                    for (let i = 0; i < dash.data.newPickup.length; i++) {
                        const e = dash.data.newPickup[i];
                        ntable.html('');
                        var r = row.clone();
                        r.find('.index').text(i + 1);
                        r.find('.sname').text(e.student.name);
                        r.find('.sclass').text(e.student.class);
                        r.find('.sgrade').text(e.student.grade);
                        r.find('.gname').text(e.guardian.name);
                        r.find('.gphone').text(e.guardian.phone);
                        r.find('.pdate').text(app.displayFormat(new Date(e.date)));
                        r.addClass('blink_me');
                        ntable.append(r);
                        await app.delay(5000);
                    }
                    await getData();
                    isFetch = true;
                }, 100);
            } else {
                isFetch = false;
                ntable.html('');
                ntable.append(emptyRow);
            }

            if (dash.data.showPickup && dash.data.showPickup.length) {
                dash.data.showPickup.forEach((e, i) => {
                    var r = row.clone();
                    r.find('.index').text(i + 1);
                    r.find('.sname').text(e.student.name);
                    r.find('.sclass').text(e.student.class);
                    r.find('.sgrade').text(e.student.grade);
                    r.find('.gname').text(e.guardian.name);
                    r.find('.gphone').text(e.guardian.phone);
                    r.find('.pdate').text(app.displayFormat(new Date(e.date)));
                    table.append(r);
                });
            } else {
                table.append(emptyRow);
            }

        } else {
            table.append(emptyRow);
            ntable.append(emptyRow);
        }
        if (!isFetch) {
            await app.delay(5000);
            await getData();
        }
    }

    getData();
})