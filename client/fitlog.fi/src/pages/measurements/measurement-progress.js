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
          this.end = new Date();
          this.start = moment(this.end).subtract(months, 'months').toDate();
          this.loadData();
        },
        loadData(){
            api.getMeasurementHistory(this.measure.id, this.start, this.end).then(response => {
                var data = response.data;
                if(data.length == 0){
                  this.data = undefined;
                    return;
                }
                this.tableData = data;
                
                var labels = data.map(d => new Date(d.time));
                var values = data.map(d => d.value);
                var start = moment(data[0].time).startOf('day');
                var end = moment(data[data.length-1].time).endOf('day');

                this.data = {
                  labels,
                  datasets: [  
                    {
                      ...graph.datasets[0],
                      type: 'line',
                      data: values

                    }
                  ]
                };

                this.options = {
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
                        labelString: this.formatUnit(this.measure.unit),
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
                        return this.formatDateTime(time);
                      }
                    }
                  }
                };
                //this.data = data.map(d =>{return  {  t: new Date(d.time), y: d.value}});
            }).catch(() => {
              this.notifyError(this.$t('fetchFailed'));
            }).finally(() => {
              this.$store.commit(constants.LOADING_DONE);
            });
            
            
        }
    },
    created() {
      this.end = new Date();
      this.start = moment(this.end).subtract(6,'month').toDate();

      api.listMeasures().then(response => {
        var measures = response.data;
        this.measures = measures.map(m => {return {...m, label: m.name, value: m }});
        var measureId = this.$route.params.measureId;
        this.measure = this.measures.find(m => m.id == measureId);
        
        this.loadData();
      });
        
    }
}