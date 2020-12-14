new Vue({
    el: 'app',
    data: {
        fields: fields,
        sort: {
            field: null,
            descending: false
        },
        results: results
    },
    computed: {
        show_results: function () {
            return this.results.length > 0;
        },
        sorted_results: function () {
            if (!this.sort.field)
                this.sort.field = this.fields[0];
            const sorted = this.results.concat().sort((r1, r2) => {
                const v1 = this.sort.field.value(r1);
                const v2 = this.sort.field.value(r2);
                return v1 > v2 ? 1
                    : (v1 < v2 ? -1
                        : 0);
            });
            return this.sort.descending ? sorted.reverse() : sorted;
        }
    },
    methods: {
        toggleSort: function (field, object) {
            this.sort.descending = (field === this.sort.field) ? !this.sort.descending : false;
            this.sort.field = field;
            this.sort.object = object;
        }
    }
});