<template>
<layout >

  <span slot="title">{{ $t('profile') }}</span>

  <div slot="toolbar"></div>
    <q-page class="q-pa-sm">
        <q-tabs v-model="tab">
      <q-tab slot="title" name="tab-1" :label="$t('basicInformation')" />
      <q-tab slot="title" name="tab-2" :label="$t('logins')" />
      <q-tab slot="title" name="tab-3" :label="$t('delete')" />
      
          <q-tab-pane name="tab-1">
              <div class="row q-ma-sm">
                    <div class="col">
                        <q-datetime v-model="dob" :float-label="$t('dob')" type="date" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" />
                    </div>
                </div>
                <div class="row q-ma-sm">
                    <div class="col">
                        <q-select v-model="gender" :float-label="$t('gender')" :options="genders"></q-select>
                    </div>
                </div>
                <div class="row q-ma-sm">
                    <div class="col">
                        <q-input type="number" v-model="height" :float-label="$t('height')" suffix="cm" />
                    </div>
                </div>
                <div class="row q-ma-sm">
                    <div class="col">
                        <q-input type="number" min="1" class="form-control input-10" v-model="weight" :float-label="$t('weight')" suffix="kg"/>
                    </div>
                </div>
                <div class="row q-ma-sm">
                    <div class="col">
                        <q-input type="number" min="1" v-model="rmr" @blur="rmrSpecified=true" :suffix="'kcal/' + $t('day')" :float-label="rmrLabel" /> 
                    </div>
                </div>
                <hr />
                <div class="row q-ma-sm">
                    <div class="col-sm-6">
                        <q-btn glossy color="primary" @click="save" :label="$t('save')"></q-btn>
                    </div>
                </div>
          </q-tab-pane>
          <q-tab-pane name="tab-2">
              <div class="row q-ma-sm">
                    <div class="col">
                        <q-input type="text" class="form-control" v-model="username" @blur="checkUsername" :readonly="hasPassword" :float-label="$t('username')" />
                    </div>
                </div>
                <div class="row q-ma-sm" v-if="hasPassword">
                    <div class="col">
                        <q-input type="password" class="form-control" v-model="oldPassword" :float-label="$t('oldPassword')" />
                    </div>
                </div>
                <div class="row q-ma-sm">
                    <div class="col">
                        <q-input type="password" class="form-control" v-model="newPassword" @blur="checkPassword" :float-label="$t('newPassword')" />
                    </div>
                </div>
                <div class="row q-ma-sm">
                    <div class="col">
                        <q-input type="password" class="form-control" v-model="newPassword2" @blur="checkPassword" :float-label="$t('confirmNewPassword')" />
                    </div>
                </div>
                <div class="row q-ma-sm">
                    <div class="col">
                        <q-btn glossy @click="updateLogin" :disabled="!isValid" :label="$t('save')"></q-btn>
                    </div>
                </div>
                <hr />
                <div class="row q-ma-sm">
                    <div class="col-6">
                        <q-btn glossy @click="connectFacebook" :disabled="hasFacebook" :label="$t('connectFacebook')"></q-btn>
                    </div>
                    <div class="col-6">
                        <q-btn glossy @click="connectGoogle" :disabled="hasGoogle" :label="$t('connectGoogle')"></q-btn>
                    </div>
                </div>
          </q-tab-pane>
          <q-tab-pane name="tab-3">
            <div class="row q-ma-sm">
                <div class="col">
                    <q-btn glossy v-if="deleteStarted" @click="deleteProfile" :label="$t('confirmDeletion')"></q-btn>
                    <q-btn glossy v-else @click="deleteStarted=true" :label="$t('deleteAccount')"></q-btn>
                </div>
            </div>
          </q-tab-pane>
      
    </q-tabs>
    </q-page>
    </layout>
</template>

<script src="./profile.js">
</script>

<style scoped>
</style>