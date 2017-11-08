<template>
    <div>
        <section class="content-header">
          <h1 v-if="type == 'Bug'">{{ $t("bugs") }}</h1>
          <h1 v-if="type == 'Improvement'">{{ $t("improvements") }}</h1>
        </section>
        <section class="content">
          <div class="row">
            <div class="col-sm-12">
                <button class="btn btn-primary" @click="createFeedback">
                    <span v-if="type == 'Bug'">{{ $t("reportBug") }}</span>
                    <span v-if="type == 'Improvement'">{{ $t("askImprovement") }}</span>
                </button>
            </div>
          </div>
        <div class="row">
            <div class="col-sm-12">
                <br />
            </div>
          </div>
          <div class="row">
            <div class="col-sm-12">
                <template v-for="item in itemsSorted">
                    <div class="box box-solid" :class="{voted: userHasVoted(item.id)}">
                        <div class="box-header with-border">
                            <h3 class="box-title">{{ item.title }}</h3>
                        </div>
                        <div class="box-body">
                            <div class="row">
                                <div class="col-sm-12">
                                    {{ item.description }}
                                    <template v-if="userHasVoted(item.id)">
                                        <a class="btn btn-app pull-right" :title="$t('youHaveVoted')">
                                            <span class="badge bg-red">{{ item.score }}</span>
                                            <i class="fa fa-thumbs-o-up"></i>
                                        </a>
                                    </template>
                                    <template v-else>
                                        <button class="btn btn-app pull-right" @click="vote(item)" :disabled="item.locked">
                                            <span class="badge bg-red">{{ item.score }}</span>
                                            <i class="fa fa-thumbs-o-up"></i> {{ $t('vote') }}
                                        </button>
                                    </template>
                                </div>
                            </div>
                            <div class="row" v-if="item.adminComment">
                                <div class="col-sm-12">
                                    <pre>{{ item.adminComment }}</pre>
                                </div>
                            </div>
                        </div>
                    </div>
                </template>
            </div>
        </div>
          
        </section>
  </div>
</template>

<script src="./feedback.js">
</script>

<style scoped>
    .box.voted i { color: blue; }
</style>