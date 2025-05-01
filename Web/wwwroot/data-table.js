function communityEmoji(stars) {
	return `<div class="community d-flex justify-content-center">
				<span class="group">`
		+ (stars.GroupRun
			? `<span class="star" title="Group Run">
					<i aria-hidden="true" class="fas fa-running"></i>
					<i aria-hidden="true" class="fas fa-running"></i>
					<i aria-hidden="true" class="fas fa-star"></i>
				</span>`
			: '')
		+ `</span>
			<span class="group">`
		+ (stars.Story
			? `<span class="star" title="Story">
					<i aria-hidden="true" class="far fa-file-alt"></i>
					<i aria-hidden="true" class="fas fa-star"></i>
				</span>`
			: '')
		+ `</span>
		</div>`;
}

new Vue({
	data: {
		fields: fields,
		sort: {
			field: null,
			descending: false
		},
		rows: rows,
		row_style: typeof (row_style) !== 'undefined' ? row_style : null
	},
	computed: {
		show_table: function () {
			return this.rows.length > 0;
		},
		fields_to_show: function () {
			return this.fields.filter(f => f.show === undefined || f.show === true);
		},
		sorted_rows: function () {
			if (!this.sort.field)
				this.sort.field = this.fields[0];
			const sorted = this.rows.concat().sort((r1, r2) => {
				const v1 = this.sort.field.sort ? this.sort.field.sort(r1) : this.sort.field.value(r1);
				const v2 = this.sort.field.sort ? this.sort.field.sort(r2) : this.sort.field.value(r2);
				return v1 > v2 ? 1
					: (v1 < v2 ? -1
						: (this.sort.field.descending ? r2.Rank?.Value - r1.Rank?.Value : r1.Rank?.Value - r2.Rank?.Value));
			});
			return this.sort.descending ^ this.sort.field.descending ? sorted.reverse() : sorted;
		}
	},
	methods: {
		toggleSort: function (field, object) {
			this.sort.descending = (field === this.sort.field) ? !this.sort.descending : false;
			this.sort.field = field;
			this.sort.object = object;
		}
	}
}).$mount('app');