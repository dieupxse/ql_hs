window.loginInfo = null;
class Api {

    constructor(baseUrl, tokenKen = 'TOKEN') {
        this.baseUrl = baseUrl;
        this.tokenKey = tokenKen;
    }
    baseUrl = '';
    tokenKey = 'TOKEN';
    headers = {
        'Content-Type': 'application/json'
    }

    getUrl (url) {
        if (url.startsWith('http')) {
            return url;
        } else {
            if (this.baseUrl.endsWith('/')) this.baseUrl = this.baseUrl.substring(0, this.baseUrl.length - 1);
            if (url.startsWith('/')) url = url.substring(1, url.length);
            return this.baseUrl + '/'+ url;
        }
    }

    getHeaders (headers) {
        var currentHeader = this.headers;
        var h = { ...currentHeader, ...headers };
        console.log(h);
        let token = localStorage.getItem(this.tokenKey)
        console.log(token);
        if (token) {
            h['Authorization'] = 'Bearer ' + token;
        }
        return h;
    }

    async get (url, headers = {}) {
        return await this.call(url, 'GET', {}, headers);
    }
    async post (url, data, headers = {}) {
        return await this.call(url, 'POST', data, headers);
    }
    async put (url, data, headers = {}) {
        return await this.call(url, 'PUT', data, headers);
    }

    async delete (url, data, headers = {}) {
        return await this.call(url, 'DELETE', data, headers);
    }

    async call (url, method, data, headers) {
        let resp;
        url = this.getUrl(url);
        headers = this.getHeaders(headers);
        try {
            var obj = {
                method: method,
                headers
            }
            if (method !== 'GET' && method !== 'HEAD') {
                obj['body'] = JSON.stringify(data);
            }
            resp = await fetch(url, obj);
        } catch (e) {
            console.error('calling api error', e);
        }
        if (resp?.ok) {
            try {
                return await resp.json();
            } catch (ex) { 
                return {
                    errorCode: resp.status,
                    errorMessage: resp.text
                }
            }
            
        } else {
            console.log(`HTTP Response Code: ${resp?.status}`)
            return null;
        }
    }
}
class App {
    api = new Api();
    constructor(baseUrl) {
        this.api.baseUrl = baseUrl;
    }
    getLoginAccount() {
        return window.loginInfo?.data
    }
    isPage(page) {
        const currentUrl = window.location.href.toLowerCase();
        console.log(currentUrl);
        return currentUrl.endsWith(page);
    }

    goTo (url) {
        window.location.href = url;
    }

    async getLoginInfo () {
        return await this.api.get('rest/current-user-info');
    }

    displayFormat(date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var hour = date.getHours();
        var min = date.getMinutes();
        var sec = date.getSeconds();
        var dateString = `${day < 10 ? '0' + day : day}-${month < 10 ? '0' + month : month}-${year} ${hour < 10 ? '0' + hour : hour}:${min < 10 ? '0' + min : min}:${sec < 10 ? '0' + sec : sec} `;
        return dateString;
    }

    async checkLogin() {
        var loginInfo = await app.getLoginInfo();
        window.loginInfo = loginInfo;
        if (loginInfo == null && !this.isPage('dang-nhap.html')) {
            this.goTo('/dang-nhap.html');
            return;
        } else if (loginInfo != null) {
            if (this.isPage('dang-nhap.html')) {
                if (['GUARDIAN'].includes(loginInfo.data.account.role)) {
                    this.goTo('don-hoc-sinh.html');
                    return;
                } else if (['MONITOR'].includes(loginInfo.data.account.role)) {
                    this.goTo('index.html');
                    return;
                } else if (['ADMIN', 'ROOT'].includes(loginInfo.data.account.role)) {
                    this.goTo('quan-ly.html');
                    return;
                }
            }
        }
        if (loginInfo) {
            if (['ROOT', 'ADMIN'].includes(loginInfo?.data?.account?.role)) {
                $('.manage-menu').removeClass('d-none');
            } else {
                $('.manage-menu').addClass('d-none');
            }
        }
        $('body').removeClass('d-none');
        return loginInfo;
    }

    checkPageAllow(srole, trole) {
        if (trole.includes(srole)) return;
        switch (srole) {
            case 'GUARDIAN':
                this.goTo('don-hoc-sinh.html');
                break;
            case 'MONITOR':
                this.goTo('index.html');
                break;
            case 'ADMIN':
            case 'ROOT':
                this.goTo('quan-ly.html');
                break;
            default:
                this.goTo('dang-nhap.html');
                break;
        }
    }

    delay(time) {
        return new Promise(resolve => setTimeout(resolve, time));
    }

    getPagination(current = 1, pageSize = 5, rowPerPage = 20, total, totalPage) {
            // TODO Auto-generated method stub
        var page = "";
        var start = 0;
        var pageNumber = 1;
        var i = 1;
        if (current == 0) current = 1;
        page += '<nav aria-label="Page navigation">';
        page += "<ul class=\"pagination\">";
        page+= "<li class=\"page-item disabled\"><a class='page-link'>Từ " + ((current - 1) * rowPerPage + 1) + " đến " + ((current * rowPerPage) > total ? total : (current * rowPerPage)) + " trong tổng số " + total + "</a></li>";
        if (totalPage > 1) {
            if (current <= totalPage) {
                if (current == 1) {
                    pageNumber = pageSize;
                    if (pageNumber > totalPage) pageNumber = totalPage;
                    start = 1;
                }
                else {
                    page += "<li class='page-item'><a href='#' class='page-link' data-page='1' id='page-1'>&laquo;&laquo;</a></li>";
                    page += "<li class='page-item'><a href='#' class='page-link' data-page='" + (current - 1) + "' id=\"page-" + (current - 1) + "\">&laquo;</a></li>";
                    if ((totalPage - current) < (pageSize / 2)) {
                        start = (totalPage - pageSize) + 1;
                        if (start <= 0) start = 1;
                        pageNumber = totalPage;
                    }
                    else {
                        start = current - (pageSize / 2);
                        if (start <= 0) start = 1;
                        pageNumber = current + (pageSize / 2);
                        if (totalPage < pageNumber) {
                            pageNumber = totalPage;
                        }
                        else if (pageNumber < pageSize) {
                            pageNumber = pageSize;
                        }
                    }
                }
                i = start;
                while (i <= pageNumber) {
                    if (i == current) {
                        page += "<li class=\"page-item active\"><a class='page-link' id=\"page-" + (i) + "\">" + i + "</a></li>";
                    }
                    else {
                        page += "<li class='page-item'><a href='#' class='page-link' data-page='"+i+"' id=\"page-" + i + "\">" + i + "</a></li>";
                    }
                    i++;
                }
                if (current < totalPage) {
                    page += "<li class='page-item'><a href='#' class='page-link' data-page='" + (current + 1) + "' id=\"page-" + (current + 1) + "\">&raquo;</a></li>";
                    page += "<li class='page-item'><a href='#' class='page-link' data-page='" + (totalPage) + "' id=\"page-" + (totalPage) + "\">&raquo;&raquo;</a></li>";
                }
            }
            page += "</ul></nav>";
        }
        return page;
    }

}
var app = new App('http://49.156.53.70:8888/api/'); //server
//var app = new App('http://localhost:38379/api/'); //localhost

$(async function () {
    $('#logoutBtn').click(async function (e) {
        e.preventDefault();
        localStorage.removeItem(app.api.tokenKey);
        app.goTo('/dang-nhap.html');
    });
    setInterval(() => {
        var dateString = app.displayFormat(new Date());
        $('#current-date').text(dateString);
    }, 1000);
})