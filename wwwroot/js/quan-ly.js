$(async function () {
    let loginInfo = await app.checkLogin();
    const rowPerPage = 10;
    app.checkPageAllow(loginInfo?.data?.account?.role, ['ADMIN', 'ROOT']);
    $('#accountName').text(`Xin chào, ${loginInfo?.data?.account?.username}`);
    //khởi tạo masking input 
    var inputElements = document.querySelectorAll("input[data-format]");
    inputElements.forEach(input => {
        let m = new IMask(input, {
            mask: input.getAttribute("data-format"),
            pattern: input.getAttribute("pattern"),
        });
    });
    var staBlock = $('#sta-block');
    var staItem = staBlock.find('.sta-item');
    await getSta();

    async function getSta() {
        var sta = await app.api.get('/Manage/statictis');
        var html = '';
        staItem.find('h3').text('Phụ huynh');
        staItem.find('h1').text(sta.data.totalGuardian);
        html += staItem.prop('outerHTML');
        staItem.find('h3').text('Học sinh');
        staItem.find('h1').text(sta.data.totalStudent);
        html += staItem.prop('outerHTML');
        staItem.find('h3').text('Tài khoản');
        staItem.find('h1').text(sta.data.totalAccount);
        html += staItem.prop('outerHTML');
        staBlock.html(html);
    }

    var tablePh = $('#table-ph');
    var tableRowPh = tablePh.find('#tb-row-ph');
    await getGuardian(page = 1);
    async function getGuardian() {
        var guardians = await app.api.get(`/Manage/guardian?page=${page}&rowPerPage=${rowPerPage}`);
        var paging = $('#paging-ph');
        var pageString = app.getPagination(page, 5, guardians.meta.rowPerPage, guardians.meta.totalItem, guardians.meta.totalPage)
        paging.html(pageString);
        paging.find('.page-link').off('click').on('click', async function (e) {
            e.preventDefault();
            var page = $(this).attr('data-page');
            if (page) {
                await getGuardian(page);
            }
        });
        var html = '';
        guardians.data.forEach((e, i) => {
            var index = (page - 1) * rowPerPage + (i + 1);
            var item = tableRowPh;
            item.find('.name').text(e.name);
            item.find('.phone').text(e.phone);
            item.find('.address').text(e.address);
            item.find('.index').text(index);
            // Binding data để hiển thị popup update thông tin phụ huynh
            item.find('.btn-warning').attr('data-id', e.id);
            item.find('.btn-warning').attr('data-bs-toggle', 'modal');
            item.find('.btn-warning').attr('data-bs-target', '#modal-ph');
            item.find('.btn-warning').attr('data-json', JSON.stringify(e));

            //Binding data để hiển thị popup thêm thông tin học sinh
            item.find('.btn-primary').attr('data-bs-toggle', 'modal');
            item.find('.btn-primary').attr('data-bs-target', '#modal-hs');
            item.find('.btn-primary').attr('data-json', JSON.stringify(e));

            item.find('.btn-danger').attr('data-json', JSON.stringify(e));
            item.find('.btn-danger').attr('data-id', e.id);
            html += item.prop('outerHTML');
        });
        tablePh.html(html);
        var modalPh = document.getElementById('modal-ph');
        if (modalPh) {
            modalPh.removeEventListener('shown.bs.modal', () => { }, false);
            modalPh.addEventListener('shown.bs.modal', (e) => {
                const button = $(e.relatedTarget);
                const data = JSON.parse(button.attr('data-json'));
                Object.keys(data).forEach((key) => {
                    $(modalPh).find(`#${key}`).val(data[key]);
                })
                $(modalPh).find('#phone').attr('readonly', 'true');
                $(modalPh).find('.btn-save-ph').attr('data-id', data.id);
            });
        }
        $(modalPh).find('.btn-save-ph').off('click').on('click', async function (e) {
            e.preventDefault();
            var data = {};
            var id = $(this).attr('data-id');
            var items = $(modalPh).find('.data-item');
            for (let item of items) {
                var et = $(item);
                data[et.attr('id')] = et.val();
            }
            var res = await app.api.put(`/Manage/guardian/${id}`, data);
            if (res && res.errorCode == 200) {
                $(modalPh).modal('hide');
                alert('Lưu thông tin thành công!');
                await getGuardian();
            } else {
                alert('Đã xảy ra lỗi. Vui lòng thử lại');
            }
        })
    }

    var tableHs = $('#table-hs');
    var tableRowHs = tableHs.find('#tb-row-hs');
    await getStudent();
    async function getStudent(page = 1) {
        var students = await app.api.get(`/Manage/student?page=${page}&rowPerPage=${rowPerPage}`);
        var paging = $('#paging-hs');
        var pageString = app.getPagination(page, 5, students.meta.rowPerPage, students.meta.totalItem, students.meta.totalPage)
        paging.html(pageString);
        paging.find('.page-link').off('click').on('click', async function (e) {
            e.preventDefault();
            var page = $(this).attr('data-page');
            if (page) {
                await getStudent(page);
            }
        });
        var html = '';
        students.data.forEach((e, i) => {
            var index = (page - 1) * rowPerPage + (i + 1);
            var item = tableRowHs;
            item.find('.name').text(e.name);
            item.find('.class').text(e.class);
            item.find('.grade').text(e.grade);
            item.find('.index').text(index);
            item.find('.btn-warning').attr('data-id', e.id);
            item.find('.btn-warning').attr('data-bs-toggle', 'modal');
            item.find('.btn-warning').attr('data-bs-target', '#modal-hs');
            item.find('.btn-warning').attr('data-json', JSON.stringify(e));
            item.find('.btn-danger').attr('data-json', JSON.stringify(e));
            item.find('.btn-danger').attr('data-id', e.id);
            html += item.prop('outerHTML');
        });
        tableHs.html(html);
        var modalHs = document.getElementById('modal-hs');
        if (modalHs) {
            modalHs.removeEventListener('shown.bs.modal', () => { }, false);
            modalHs.addEventListener('shown.bs.modal', (e) => {
                const button = $(e.relatedTarget);
                const id = button.attr('data-id');
                const data = JSON.parse(button.attr('data-json'));
                var items = $(modalHs).find('.data-item');
                for (let item of items) {
                    var et = $(item);
                    et.val('');
                }
                $(modalHs).find('.btn-save-hs').removeAttr('data-id');
                $(modalHs).find('.btn-save-hs').removeAttr('data-g-id');
                if (id) {
                    Object.keys(data).forEach((key) => {
                        $(modalHs).find(`#${key}`).val(data[key]);
                    })
                    $(modalHs).find('.btn-save-hs').attr('data-id', id);
                    $(modalHs).find('.modal-title').text('Cập nhật thông tin học sinh ' + data.name);
                } else {
                    $(modalHs).find('.btn-save-hs').attr('data-g-id', data.id);
                    $(modalHs).find('.modal-title').text('Thêm học sinh của phụ huynh ' + data.name);
                }
            });
        }
        $(modalHs).find('.btn-save-hs').off('click').on('click', async function (e) {
            e.preventDefault();
            var data = {};
            const id = $(this).attr('data-id');
            const gid = $(this).attr('data-g-id');
            var items = $(modalHs).find('.data-item');
            for (let item of items) {
                var et = $(item);
                data[et.attr('id')] = et.val();
            }
            if (gid) {
                data['guardianId'] = +gid;
            }
            var res;
            if (id) {
                res = await app.api.put(`/Manage/student/${id}`, data);
            } else {
                res = await app.api.post(`/Manage/student`, data);
            }
            
            if (res && res.errorCode == 200) {
                $(modalHs).modal('hide');
                alert('Lưu thông tin thành công!');
                await getStudent();
                if (!id) {
                    await getGuardian();
                }
            } else {
                alert('Đã xảy ra lỗi. Vui lòng thử lại');
            }
        })
    }

    var tableAc = $('#table-ac');
    var tableRowAc = tableAc.find('#tb-row-ac');
    await getAccount();
    async function getAccount(page=1) {
        var acc = await app.api.get(`/Manage/account?page=${page}&rowPerPage=${rowPerPage}`);
        var paging = $('#paging-ac');
        var pageString = app.getPagination(page, 5, acc.meta.rowPerPage, acc.meta.totalItem, acc.meta.totalPage);
        paging.html(pageString);
        paging.find('.page-link').off('click').on('click', async function (e) {
            e.preventDefault();
            var page = $(this).attr('data-page');
            if (page) {
                await getAccount(page);
            }
        });
        var html = '';
        acc.data.forEach((e, i) => {
            var index = (page - 1) * rowPerPage + (i + 1);
            var item = tableRowAc;
            item.find('.username').text(e.username);
            item.find('.role').text(e.role);
            item.find('.date').text(app.displayFormat(new Date(e.createdDate)));
            item.find('.index').text(index);
            item.find('.btn-warning').attr('data-id', e.id);
            item.find('.btn-warning').attr('data-bs-toggle', 'modal');
            item.find('.btn-warning').attr('data-bs-target', '#modal-ac');
            item.find('.btn-warning').attr('data-json', JSON.stringify(e));
            item.find('.btn-danger').attr('data-json', JSON.stringify(e));
            item.find('.btn-danger').attr('data-id', e.id);
            html += item.prop('outerHTML');
        });
        tableAc.html(html);
        var modalAc = document.getElementById('modal-ac');
        if (modalAc) {
            modalAc.removeEventListener('shown.bs.modal', () => { }, false);
            modalAc.addEventListener('shown.bs.modal', (e) => {
                const button = $(e.relatedTarget);
                const dataJson = button.attr('data-json');
                const id = button.attr('data-id');
                var items = $(modalAc).find('.data-item');
                var items = $(modalAc).find('.data-item');
                for (let item of items) {
                    var et = $(item);
                    et.val('');
                }
                if (id && dataJson) {
                    const data = JSON.parse(dataJson);
                    Object.keys(data).forEach((key) => {
                        $(modalAc).find(`#${key}`).val(data[key]);
                    })
                    $(modalAc).find('#username').attr('readonly', 'true');
                    $(modalAc).find('.btn-save-ac').attr('data-id', id);
                }
            });
        }
        $(modalAc).find('.btn-save-ac').off('click').on('click', async function (e) {
            e.preventDefault();
            var data = {};
            var id = $(this).attr('data-id');
            var items = $(modalAc).find('.data-item');
            for (let item of items) {
                var et = $(item);
                data[et.attr('id')] = et.val();
            }
            var res;
            if (id) {
                res = await app.api.put(`/Manage/account/${id}`, data);
            } else {
                res = await app.api.post(`/Manage/account`, data);
            }
            if (res && res.errorCode == 200) {
                $(modalAc).modal('hide');
                alert('Lưu thông tin thành công!');
                await getAccount();
            } else {
                alert('Đã xảy ra lỗi. Vui lòng thử lại');
            }
        })
    }

    var modalInfo = document.getElementById('modal-create-info');
    if (modalInfo) {
        modalInfo.removeEventListener('shown.bs.modal', () => { }, false);
        modalInfo.addEventListener('shown.bs.modal', (e) => {
            const button = $(e.relatedTarget);
            var items = $(modalInfo).find('.data-item');
            for (let item of items) {
                var et = $(item);
                et.val('');
            }
        });
    }
    $(modalInfo).find('.btn-save-info').off('click').on('click', async function (e) {
        e.preventDefault();
        var data = {};
        var items = $(modalInfo).find('.data-item');
        for (let item of items) {
            var et = $(item);
            data[et.attr('id')] = et.val();
        }
        var res = await app.api.post(`/Manage/create-student-info`, data);
        if (res && res.errorCode == 200) {
            $(modalInfo).modal('hide');
            alert('Lưu thông tin thành công!');
            await getAccount();
            await getStudent();
            await getGuardian();
            await getSta();
        } else {
            alert('Đã xảy ra lỗi. Vui lòng thử lại');
        }
    })

    $('.btn-del').off('click').on('click', async function (e) {
        e.preventDefault();
        var id = $(this).attr('data-id');
        var url = $(this).attr('data-url');
        var type = $(this).attr('data-type');
        var data = JSON.parse($(this).attr('data-json'));
        var apiUrl = `${url}/${id}`;
        if (type == 'account' && data.role == 'ROOT') {
            alert('Không thể xóa tài khoản root');
        } else {
            if (confirm('Bạn có chắc muốn xóa thông tin này không?')) {
                var res = await app.api.delete(apiUrl);
                if (res && res.errorCode == 200) {
                    alert('Xóa thông tin thành công!');
                    await getAccount();
                    await getStudent();
                    await getGuardian();
                    await getSta();
                } else {
                    alert('Đã xảy ra lỗi. Vui lòng thử lại');
                }
            }
        }
    });
})