"use strict";
var KTPasswordResetGeneral = function () {
	var e, t, r;
	return {
		init: function () {
			e = document.querySelector("#kt_password_reset_form"), t = document.querySelector("#kt_password_reset_submit"), r = FormValidation.formValidation(e, {
				fields: {
					Email: {
						validators: {
							notEmpty: {
								message: "Email address is required"
							},
							emailAddress: {
								message: "The value is not a valid email address"
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger,
					bootstrap: new FormValidation.plugins.Bootstrap5({
						rowSelector: ".fv-row",
						eleInvalidClass: "",
						eleValidClass: ""
					})
				}
			}), t.addEventListener("click", function (i) {
				i.preventDefault(), r.validate().then(function (r) {
					if ("Valid" == r) (t.setAttribute("data-kt-indicator", "on"), t.disabled = !0, e.submit());
				})
			})
		}
	}
}();
KTUtil.onDOMContentLoaded(function () {
	KTPasswordResetGeneral.init()
});