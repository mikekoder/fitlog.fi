export default {
    isLoggedIn() {
        return sessionStorage.getItem('userid') && true;
    },
    setUserInfo(userid){
        sessionStorage.setItem('userid',userid);
    },
    clearUserInfo(){
        sessionStorage.removeItem('userid');
    }
}
