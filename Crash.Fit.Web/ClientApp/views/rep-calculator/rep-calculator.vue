<template>
    <div v-if="!loading">
        <div>
            <section class="content-header">
                <h1>{{ $t("repCalculator") }}</h1>
            </section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <input type="number" min="1" step="1" v-model="reps" :placeholder="$t('reps')" /> x
                        <input type="number" min="0" step="2.5" v-model="weights" :placeholder="$t('weights')" />
                        <button class="btn btn-primary" @click="calculate">{{ $t('calculate') }}</button>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12">
                        <table class="table">
                            <thead>
                                <tr>
                                    <th>{{ $t('reps') }}</th>
                                    <th>{{ $t('average') }}</th>
                                    <th>Epley<sub>*</sub></th>
                                    <th>Brzycki<sub>*</sub></th>
                                    <th>Lander<sub>*</sub></th>
                                    <th>Lombardi<sub>*</sub></th>
                                    <th>Mayhew<sub>*</sub></th>
                                    <th>OConner<sub>*</sub></th>
                                    <th>Wathan<sub>*</sub></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="result in results">
                                    <td>{{ result.reps }}</td>
                                    <td>{{ formatDecimal(result.avg, 2) }}</td>
                                    <td>{{ formatDecimal(result.Epley, 2) }}</td>
                                    <td>{{ formatDecimal(result.Brzycki, 2) }}</td>
                                    <td>{{ formatDecimal(result.Lander, 2) }}</td>
                                    <td>{{ formatDecimal(result.Lombardi, 2) }}</td>
                                    <td>{{ formatDecimal(result.Mayhew, 2) }}</td>
                                    <td>{{ formatDecimal(result.OConner, 2) }}</td>
                                    <td>{{ formatDecimal(result.Wathan, 2) }}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12">
                        {{ $t('repCalculatorInfo') }} <a href="https://en.wikipedia.org/wiki/One-repetition_maximum">"One-repetition maximum"</a>
                    </div>
                </div>
            </section>
        </div>
    </div>
</template>

<script src="./rep-calculator.js">
</script>

<style scoped>
    table
    {
        width: auto;
        table-layout: fixed; 
        /*width: 100%;*/
    }
    table td 
    {
        padding-bottom: 0px;
    }
    table td span
    {
        margin: 5px;
    }
</style>