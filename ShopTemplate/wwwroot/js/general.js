"use strict";
var e, t, a, s, r = function () {
    return 100 === s.getScore()
};
var fields = {
    Email: {
        validators: {
            notEmpty: {
                message: "Email address is required"
            },
            emailAddress: {
                message: "The value is not a valid email address"
            }
        }
    },
    Password: {
        validators: {
            notEmpty: {
                message: "The password is required"
            },
            callback: {
                message: "Please enter valid password of 8 or more characters",
                callback: function (e) {
                    //if (e.value.length > 0) return r();
                    if (e.value.length < 8 && e.value.length > 0) return false;
                    else return true;
                }
            }
        }
    }
};
var KTSignupGeneral = function () {

    return {
        init: function () {
            e = document.querySelector("#kt_sign_up_form"), t = document.querySelector("#kt_sign_up_submit"), s = KTPasswordMeter.getInstance(e.querySelector('[data-kt-password-meter="true"]')), a = FormValidation.formValidation(e, {
                fields: fields,
                plugins: {
                    trigger: new FormValidation.plugins.Trigger({
                        event: {
                            password: !1
                        }
                    }),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: ".fv-row",
                        eleInvalidClass: "",
                        eleValidClass: ""
                    })
                }
            }), t.addEventListener("click", (function (r) {
                r.preventDefault(), a.revalidateField("Password"), a.validate().then((function (a) {
                    if ("Valid" == a) (t.setAttribute("data-kt-indicator", "on"), t.disabled = !0, e.submit());
                }))
            })), e.querySelector('input[name="Password"]').addEventListener("input", (function () {
                this.value.length > 0 && a.updateFieldStatus("Password", "NotValidated")
            }))
        }
    }
}();
KTUtil.onDOMContentLoaded((function () {
    KTSignupGeneral.init()
}));