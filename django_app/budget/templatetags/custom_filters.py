from django import template

register = template.Library()

@register.filter
def subtract(value, arg):
    """Subtrai arg de value"""
    try:
        return float(value) - float(arg)
    except (ValueError, TypeError):
        return 0
