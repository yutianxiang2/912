function Util() { }

//验证时间，形如01:03
Util.prototype.isTime = function (columnLabel, inputStrTime) {

    var ret = inputStrTime.match(/^(\d{2})(:)?(\d{2})$/);
    if (ret == null) {alert(columnLabel + ':不是正确的时间格式(hh:mm)'); return false;}
    if (ret[1]>=24 || ret[3]>=60)
    {
        alert(columnLabel + ":输入的时间无效");
        return false
    }
    return true;
}

//验证日期，形如2001-03-01
Util.prototype.isDate = function (columnLabel, inputStrDate) {
    var r = inputStrDate.match(/^(\d{4})(-|\/)(\d{2})\2(\d{2})$/);
    if (r == null) { alert(columnLabel + ':不是正确的日期格式(yyyy-mm-dd)'); return false; }
    var d = new Date(r[1], r[3] - 1, r[4]);
    if (false == (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4])) {
        alert(columnLabel + ":输入的日期无效");
        return false
    }
    return true;
}

//验证日期时间，形如 (2008-07-22 13:04:06)
Util.prototype.isDateTime = function (columnLabel, inputStrDateTime) {
    var reg = /^(\d{4})(-|\/)(\d{2})\2(\d{2}) (\d{2}):(\d{2}):(\d{2})$/;
    var r = inputStrDateTime.match(reg);
    if (r == null) { alert(columnLabel + ':不是正确的日期时间格式(yyyy-mm-dd hh:mm:ss)'); return false; }
    var d = new Date(r[1], r[3] - 1, r[4], r[5], r[6], r[7]);
    if (false == (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4] && d.getHours() == r[5] && d.getMinutes() == r[6] && d.getSeconds() == r[7])) {
        alert(columnLabel + ":输入的日期时间无效");
        return false;
    }
    return true;
}

//验证字符串
Util.prototype.isValidString = function (columnLabel, inputString) { }

//验证Email
Util.prototype.isEmail = function (columnLabel, inputStrEmail) {
    var reg = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
    if (reg.test(inputStrEmail)) {
        return true;
    } else {
        alert(columnLabel + ":输入的电子邮件无效");
        return false;
    }
}

Util.prototype.isEmail2 = function (inputStrEmail) {
    var reg = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
    if (reg.test(inputStrEmail)) {
        return true;
    } else {
        return false;
    }
}

//验证货币金额
Util.prototype.isValidCurrency = function (columnLabel, inputStrCurrency) {
    var reg = /^\d+\.?\d{0,2}$/;
    if (reg.test(inputStrCurrency)) {
        return true;
    }
    else {
        alert(columnLabel + ":输入的金额无效(最多两位小数)");
        return false;
    }
}

//验证折扣
Util.prototype.isValidDiscount = function (columnLabel, inputStrCurrency) {
    var reg = /^\d+\.?\d{0,2}$/;
    if (reg.test(inputStrCurrency)) {
        if (parseFloat(inputStrCurrency) > 1 || parseFloat(inputStrCurrency) <= 0) {
            alert(columnLabel + ":输入的折扣无效(0-1之间的两位小数)");
            return false;
        }
        return true;
    }
    else {
        alert(columnLabel + ":输入的折扣无效(0-1之间的两位小数)");
        return false;
    }
}


//验证正整数
Util.prototype.isValidPositiveInt = function (columnLabel, inputStrCurrency) {
    var reg = /^[1-9]*[1-9][0-9]*$/;
    if (reg.test(inputStrCurrency)) {
        return true;
    }
    else {
        alert(columnLabel + ":输入的正整数无效)");
        return false;
    }
}

//验证小数 
Util.prototype.isValidFloat = function (columnLabel, inputStrFloat) { 
    var reg = /^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/;
    if (reg.test(inputStrFloat)) {
        return true;
    }
    else {
        alert(columnLabel + ":输入的小数无效");
        return false;
    }
}

//验证整数
Util.prototype.isValidinteger = function (columnLabel, inputStrInteger) {
    var reg = /^-?\d+$/;
    if (reg.test(inputStrInteger)) { return true; }
    else {
        alert(columnLabel + ":输入的整数无效");
        return false;
    }
}

//验证自然数0，1，2，3...
Util.prototype.isValidNaturalNumber = function (columnLabel, inputStrNaturalNumber) {
    
    var r = inputStrNaturalNumber.match("^\\d+$");
    if (r == null)
    {  
        alert(columnLabel + ":请输入大于或等于0的整数");
        return false;
    }   
    else   
    {  
        return true;
    }
}


//将传入数据转换为字符串,并清除字符串中非数字与.的字符  
//按数字格式补全字符串  

Util.prototype.FormatPrice = function (num) {
    num += '';
    num = num.replace(/[^0-9|\.]/g, ''); //清除字符串中的非数字非.字符  

    if (/^0+/) //清除字符串开头的0  
        num = num.replace(/^0+/, '');
    if (!/\./.test(num)) //为整数字符串在末尾添加.00  
        num += '.00';
    if (/^\./.test(num)) //字符以.开头时,在开头添加0  
        num = '0' + num;
    num += '00';        //在字符串末尾补零  
    num = num.match(/\d+\.\d{2}/)[0];
}