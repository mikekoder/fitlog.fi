<template>
    <div>
        <section class="content-header">
          <h1 v-if="type == 'Bug'">{{ $t("bugs") }}</h1>
          <h1 v-if="type == 'Improvement'">{{ $t("improvements") }}</h1>
        </section>
        <section class="content">
          <div class="row">
            <div class="col-sm-12">
              <div class="form-group">
                <label>{{ $t('title') }}</label><br />
                <input type="text" class="form-control" v-model="title" />
              </div>
            </div>
          </div>
          <div class="row">
            <div class="col-sm-12">
              <div class="form-group">
                <label>{{ $t('description') }}</label>
                <br />
                <textarea class="form-control" v-model="description"></textarea>
              </div>
            </div>
          </div>
          <div class="row main-actions">
            <div class="col-sm-12">
              <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
              <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
              <button class="btn btn-danger" @click="deleteFeedback" v-if="id">{{ $t("delete") }}</button>
            </div>
          </div>
        </section>
  </div>
</template>

<script src="./feedback-details.js">
</script>

<style scoped>
</style>