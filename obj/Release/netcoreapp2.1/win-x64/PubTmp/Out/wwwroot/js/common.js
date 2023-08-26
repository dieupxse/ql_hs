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
    isPage (page) {
        return window.location.href.toLowerCase().endsWith(page);
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
                if (loginInfo.data.account.role == 'GUARDIAN') {
                    this.goTo('don-hoc-sinh.html');
                    return;
                } else if (['ADMIN', 'ROOT', 'MONITOR'].includes(loginInfo.data.account.role)) {
                    this.goTo('index.html');
                    return;
                }
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
            case 'ADMIN':
            case 'ROOT':
            case 'MONITOR':
                this.goTo('index.html');
                break;
            default:
                this.goTo('dang-nhap.html');
                break;
        }
    }

    delay(time) {
        return new Promise(resolve => setTimeout(resolve, time));
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