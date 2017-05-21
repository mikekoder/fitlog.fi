<template>
    <div class="wrapper">
        <header class="main-header">
            <a href="#/" class="logo">
                <span class="logo-mini"><b>C</b>F</span>
                <span class="logo-lg"><b>Crash</b>FIT</span>
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
                            <a><i class="fa fa-gear"></i> <span>Profiili</span></a>
                        </router-link>
                        <li v-if="!isLoggedIn">
                            <a href="#/luo-tunnus">Rekister&ouml;idy</a>
                        </li>
                        <li v-if="!isLoggedIn">
                            <a href="#/kirjaudu">Kirjaudu</a>
                        </li>
                        <li v-if="isLoggedIn">
                            <a @click="logout">Kirjaudu ulos</a>
                        </li>
                    </ul>
                </div>
            </nav>
        </header>
        <aside class="main-sidebar">
            <mainmenu />
        </aside>
        <div class="content-wrapper">
            <transition name="md-router">
                <router-view></router-view>
            </transition>
        </div>
        <footer class="main-footer">
            <strong>Copyright &copy; 2017 <a href="#">Mika Kolari</a></strong>
        </footer>
    </div>
</template>

<script>
    var api = require('../api');
    var auth = require('../auth');
module.exports = {
    data () {
        return {
        }
    },
    components: {
        'mainmenu': require('../components/menu')
    },
    methods: {
        logout() {
            api.logout().then(function () {
                auth.clearUserInfo();
                window.location = '/';
            });
        }
    }
}
</script>

<style scoped>
</style>