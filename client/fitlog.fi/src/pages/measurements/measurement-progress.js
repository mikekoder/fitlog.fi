import constants from '../../store/constants'
import api from '../../api'
import GraphBar from '../../components/graph-bar'
import moment from 'moment'
import graph from '../../graph'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
    components: {
        GraphBar
    },
    data () {
        return {
            measure: undefined,
            measures: [],
            start: undefined,
            end: undefined,
            data: undefined,
            options: undefined,
            tableData: []
        }
    },
    computed:{
    },
    methods: {
        showMonths(months){
            var self = this;
            self.end = new Date();
            self.start = moment(self.end).subtract(months, 'months').toDate();
            self.loadData();
        },
        loadData(){
            var self = this;
            api.getMeasurementHistory(self.measure.id, self.start, self.end).then(response => {
                var data = response.data;
                if(data.length == 0){
                    self.data = undefined;
                    return;
                }
                this.tableData = data;
                
                var labels = data.map(d => new Date(d.time));
                var values = data.map(d => d.value);
                var start = moment(data[0].time).startOf('day');
                var end = moment(data[data.length-1].time).endOf('day');

                self.data = {
                    labels,
                    datasets: [  
                      {
                        ...graph.datasets[0],
                        type: 'line',
                        data: values

                      }
                    ]
                };

                self.options = {
                    maintainAspectRatio: false,
                    legend: {
                        display: false
                    },
                    scales: {
                        xAxes:[
                            {
                                type: 'time',
                                time: {
                                    unit: 'day',
                                    displayFormats: {
                                        day: 'DD.MM.YYYY',
                                        hour: 'HH:mm'
                                    },
                                    min: start,
                                    max: end
                                }
                            }
                        ],
                        yAxes: [{
                            /*
                            scaleLabel:{
                                display: true,
                                labelString: self.formatUnit(self.measure.unit),
                            },*/
                            fill: false,
                            ticks: {
                                //max: 5,
                                min: 0,
                                //max: max,
                                //stepSize: step 
                            }
                        }]
                    },
                    tooltips:{
                        callbacks:{
                            title: function(tooltipItem, data) {
                                var time = data.labels[tooltipItem[0].index];
                                return self.formatDateTime(time);
                            }
                        }
                    }
                };
                //self.data = data.map(d =>{return  {  t: new Date(d.time), y: d.value}});
            }).catch(() => {
                self.notifyError(self.$t('fetchFailed'));
            }).finally(() => {
                self.$store.commit(constants.LOADING_DONE);
            });
            
            
        }
    },
    created() {
        var self = this;
        self.end = new Date();
        self.start = moment(self.end).subtract(6,'month').toDate();

        api.listMeasures().then(response => {
            var measures = response.data;
            self.measures = measures.map(m => {return {...m, label: m.name, value: m }});
            var measureId = self.$route.params.measureId;
            self.measure = self.measures.find(m => m.id == measureId);
            
            self.loadData();
        });
        
    }
}