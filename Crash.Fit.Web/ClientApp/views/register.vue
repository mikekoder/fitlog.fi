<template>
    <div class="register-box">
        <div class="register-logo">
            <a href="../../index2.html"><b>Admin</b>LTE</a>
        </div>
        <div class="register-box-body">
            <p class="login-box-msg">Register a new membership</p>
            <form action="../../index.html" method="post">
                <div class="form-group has-feedback">
                    <input type="email" class="form-control" placeholder="Email">
                </div>
                <div class="form-group has-feedback">
                    <input type="password" class="form-control" placeholder="Password">
                </div>
                <div class="form-group has-feedback">
                    <input type="password" class="form-control" placeholder="Retype password">
                </div>
                <div class="row">
                    <div class="col-xs-8">
                        <input type="checkbox"> I agree to the <a href="#">terms</a>
                    </div>
                    <!-- /.col -->
                    <div class="col-xs-4">
                        <button type="submit" class="btn btn-primary btn-block btn-flat">Register</button>
                    </div>
                    <!-- /.col -->
                </div>
            </form>
            <div class="social-auth-links text-center">
                <p>- OR -</p>
                <a href="#" class="btn btn-block btn-social btn-facebook btn-flat">
                    <i class="fa fa-facebook"></i> Sign up using
                    Facebook
                </a>
                <a href="#" class="btn btn-block btn-social btn-google btn-flat">
                    <i class="fa fa-google-plus"></i> Sign up using
                    Google+
                </a>
            </div>
            <a href="login.html" class="text-center">I already have a membership</a>
        </div>
        <!-- /.form-box -->
    </div>
</template>

<script>
module.exports = {
    data () { return {}},
    components: {},
    methods: { }
}
</script>

<style scoped>
    input[type=checkbox]{
        width: 20px;
        height:20px;
    }
</style>