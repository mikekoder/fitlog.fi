<template>
    <div class="wrapper">
        <header class="main-header">
            <a href="#/" class="logo">
                <span class="logo-mini"><b>fl</b></span>
                <span class="logo-lg"><b>fitlog</b></span>
            </a>
            <nav class="navbar navbar-static-top" role="navigation">
                <a href="#" class="sidebar-toggle" data-toggle="offcanvas" role="button">
                    <span class="sr-only">Toggle navigation</span>
                </a>
                <div class="navbar-custom-menu">
                    <ul class="nav navbar-nav"> <!--
                        <li class="dropdown user user-menu">
                            
 
                            <ul class="dropdown-menu">
                                <li class="user-footer">
                                    <div class="pull-left">
                                        <a href="#" class="btn btn-default btn-flat">Profile</a>
                                    </div>
                                    <div class="pull-right">
                                        <a href="#" class="btn btn-default btn-flat">Sign out</a>
                                    </div>
                                </li>
                            </ul>
                        </li>-->
                        <router-link tag="li" :to="{ name: 'profile'}" v-if="isLoggedIn">
                            <a><i class="fa fa-user"></i> <span>{{ $t("profile") }}</span></a>
                        </router-link>
                        <li v-if="!isLoggedIn">
                            <a href="#/luo-tunnus">{{ $t("register") }}</a>
                        </li>
                        <li v-if="!isLoggedIn">
                            <a href="#/kirjaudu">{{ $t("login") }}</a>
                        </li>
                        <li v-if="isLoggedIn">
                            <a @click="logout">{{ $t("logout") }}</a>
                        </li>
                    </ul>
                </div>
            </nav>
        </header>
        <aside class="main-sidebar">
            <mainmenu />
        </aside>
        <div class="content-wrapper">
            <div class="loader" v-if="loading">
                <i class="fa fa-spinner fa-spin fa-3x fa-fw"></i>
                <span class="sr-only">Loading...</span>
            </div>
            <transition name="slide-left">
                <router-view></router-view>
            </transition>
        </div>
        <footer class="main-footer">
            <strong>&copy; <a href="http://mikakolari.fi">Mika Kolari</a></strong>
        </footer>
    </div>
</template>

<script>
    import constants from '../store/constants'
    import api from '../api'

export default {
    data () {
        return {
        }
    },
    computed: {
    },
    components: {
        'mainmenu': require('../components/menu')
    },
    methods: {
        logout() {
            this.$store.dispatch(constants.LOGOUT, {
                success() {
                    window.location = '/';
                },
                failure() { }
            });
        },
        refreshTokens() {
            var self = this;

            self.$store.dispatch(constants.REFRESH_TOKEN, {
                success() { 
                    if(!self.isLoggedIn){
                        self.$store.dispatch(constants.FETCH_PROFILE, {});
                    }
                },
                failure() {
                    //toaster(self.$t('failed'));
                }
            });
        }
    },
    created() {
        var self = this;

        self.$store.dispatch(constants.FETCH_PROFILE, {
            success() { },
            failure() { }
        });
        
        setInterval(() => {
            self.refreshTokens();
        }, 60000); 
    },
    beforeRouteUpdate(to, from, next) {
        this.$store.commit(constants.LOADING);
        next();
    },
    beforeRouteLeave(to, from, next) {
        this.$store.commit(constants.LOADING);
        next();
    }
}
</script>

<style scoped>
    .loader {
        position: relative;
        top: 100px;
        left: 200px;
    }
</style>