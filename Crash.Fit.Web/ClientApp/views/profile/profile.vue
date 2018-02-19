<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t('profile') }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <ul class="nav nav-tabs">
                        <li :class="{ active: tab == 'basic' }"><a @click="tab='basic'">{{ $t('basicInformation') }}</a></li>
                        <li :class="{ active: tab == 'logins' }"><a @click="tab='logins'">{{ $t('logins') }}</a></li>
                        <li :class="{ active: tab == 'delete' }"><a @click="tab='delete'">{{ $t('delete') }}</a></li>
                    </ul>
                </div>
            </div>
            <div class="row"></div>
            <div v-if="tab == 'basic'">
                <div class="row">
                    <div class="col-sm-12">
                        <div class="form-group dob">
                            <label>{{ $t('dob') }}</label><br />
                            <select v-model="day" class="form-control input-2">
                                <option v-for="dayOption in days" :value="dayOption">
                                    {{ dayOption }}
                                </option>
                            </select>
                            <select v-model="month" class="form-control input-10">
                                <option v-for="monthOption in months" :value="monthOption">
                                    {{ monthOption.name }}
                                </option>
                            </select>
                            <select v-model="year" class="form-control input-4">
                                <option v-for="yearOption in years" :value="yearOption">
                                    {{ yearOption }}
                                </option>
                            </select>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('gender') }}</label>
                            <select class="form-control input-10" v-model="gender">
                                <option :value="undefined"></option>
                                <option :value="'male'">Mies</option>
                                <option :value="'female'">Nainen</option>
                            </select>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('height') }}</label><br />
                            <input type="number" class="form-control input-10" v-model="height" /> cm
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('weight') }}</label><br />
                            <input type="number" min="1" class="form-control input-10" v-model="weight" /> kg
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('rmr') }}<span class="estimate" v-if="rmrSpecified"><br />{{ $t('estimate') }}: {{ rmrEstimate }} kcal/{{ $t('day')}}</span></label><br />
                            <input type="number" min="1" class="form-control input-10" v-model="rmr" @blur="rmrSpecified=true" /> kcal/{{ $t('day') }}
                        </div>

                    </div>
                </div>
                <!--
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('pal') }}</label><br />
                            <select v-model="pal" class="form-control input-25">
                                <option v-for="palOption in pals" :value="palOption">
                                    {{ palOption.name }}
                                </option>
                            </select>
                        </div>

                    </div>
                </div>-->
                <hr />
                <div class="row">
                    <div class="col-sm-6">
                        <button class="btn btn-primary" @click="save">{{ $t('save') }}</button>
                    </div>
                </div>
            </div>
            <div v-if="tab == 'logins'">
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t("username") }}</label> <span class="error">{{ usernameError }}</span>
                            <input type="text" class="form-control" v-model="username" @blur="checkUsername" :disabled="hasPassword">
                        </div>
                    </div>
                </div>
                <div class="row" v-if="hasPassword">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('oldPassword') }}</label><br />
                            <input type="text" class="form-control" v-model="oldPassword" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t("newPassword") }}</label> ({{ $t("min") }} 6 {{ $t("characters") }})<span class="error">{{ passwordError}}</span>
                            <input type="password" class="form-control" v-model="newPassword" @blur="checkPassword">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t("confirmNewPassword") }}</label> <span class="error">{{ password2Error}}</span>
                            <input type="password" class="form-control" v-model="newPassword2" @blur="checkPassword">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <button class="btn btn-primary" @click="updateLogin" :disabled="!isValid">{{ $t('save') }}</button>
                    </div>
                </div>
                <hr />
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="connectFacebook" :disabled="hasFacebook">{{ $t('connectFacebook') }}</button>
                        <button class="btn btn-primary" @click="connectGoogle" :disabled="hasGoogle">{{ $t('connectGoogle') }}</button>
                    </div>
                </div>
            </div>
            <div v-if="tab == 'delete'">
                <div class="row">&nbsp;</div>
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-danger" v-if="deleteStarted" @click="deleteProfile">{{ $t("confirmDeletion") }}</button>
                        <button class="btn btn-danger" v-else @click="deleteStarted=true">{{ $t("deleteAccount") }}</button>
                    </div>
                </div>
            </div>
        </section>
    </div>
</template>

<script src="./profile.js">
</script>

<style scoped>
    .dob .form-control {
        display:initial;
    }
    input.form-control { display:initial;}
    span.error {
        color: red;
    }
    span.estimate {
        font-weight: normal;
        font-size: smaller;
    }
</style>