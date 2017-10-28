<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("nutritionGoals") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="createGoal">{{ $t("create") }}</button>
                </div>
            </div>
            <div class="row" v-if="goals.length > 0">
                <div class="col-sm-12">     
                    <table class="table" id="goal-list">
                        <thead>
                            <tr>
                                <th class="name col-text-30">{{ $t("name") }}</th>
                                <th class="col-text-10"></th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class="routine" v-for="goal in goals">
                                <td><router-link :to="{ name: 'nutrition-goal-details', params: { id: goal.id } }">{{ goal.name }}</router-link></td>
                                <td>
                                    <span v-if="goal.active">{{ $t("active") }}</span>
                                    <button class="btn btn-primary" v-if="!goal.active" @click="activate(goal)">{{ $t("activate") }}</button>
                                </td>
                                <td><button class="btn btn-danger btn-xs" @click="deleteGoal(goal)">{{ $t("delete") }}</button></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row" v-if="goals.length == 0">
                <div class="col-sm-12">
                    <br />
                    {{ $t("noNutritionGoals") }}
                </div>
            </div>
        </section>
    </div>
</template>

<script src="./nutrition-goals.js">
</script>

<style scoped>
</style>