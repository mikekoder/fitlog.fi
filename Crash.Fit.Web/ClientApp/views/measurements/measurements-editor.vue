<template>
    <div id="measurements">
        <div class="row">
            <div class="col-sm-5 col-md-3 col-lg-2">
                <div class="form-group">
                    <label>{{ $t("time") }}</label>
                    <datetime-picker class="vue-picker1" name="picker1" :value="time" @change="val => time=val"></datetime-picker>
                </div>
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-12">
                <div class="row">
                    <div class="hidden-xs col-sm-4 col-md-2"><label>{{ $t("measure") }}</label></div>
                    <div class="hidden-xs col-xs-4 col-sm-2"><label>{{ $t("value") }}</label></div>
                    <div class="hidden-xs col-xs-4 col-sm-2">&nbsp;</div>
                </div>
                <template v-for="(measurement,index) in measurements">
                    <div class="measurement row">
                        <div class="col-sm-4 col-md-2">
                            <label class="hidden-sm hidden-md hidden-lg">{{ $t("measure") }}</label>
                            <span v-if="measurement.id">{{ measurement.name }}</span>
                            <input type="text" class="form-control" v-else v-model="measurement.name" />
                        </div>
                        <div class="col-sm-4 col-md-2">
                            <label class="hidden-sm hidden-md hidden-lg">{{ $t("value") }}</label>
                            <input type="number" class="form-control" v-model="measurement.value" />
                        </div>
                        <div class="col-xs-4 col-sm-2">
                            <div>
                                <button class="btn btn-danger btn-sm" v-if="!measurement.id" @click="deleteMeasurement(index)">{{ $t("delete") }}</button>
                            </div>
                        </div>
                    </div>
                    <div class="measure-separator row hidden-sm hidden-md hidden-lg">
                        <div class="col-sm-12"><hr /></div>
                    </div>
                </template>
                <div class="row">
                    <div class="col-sm-12">&nbsp;</div>
                </div>
                <div class="row">
                    <div class="col-xs-12"><button class="btn" @click="addMeasurement">{{ $t("add") }}</button></div>
                </div>
            </div>
        </div>
        <hr />
        <div class="row main-actions">
            <div class="col-sm-12">
                <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
            </div>
        </div>
        <hr />
        <div class="row">
            <table>
                <tbody></tbody>
            </table>
        </div>
    </div>
</template>

<script src="./measurements-editor.js">
</script>
<style scoped>
    #measurements 
    {
        max-width:1200px;
    }
</style>